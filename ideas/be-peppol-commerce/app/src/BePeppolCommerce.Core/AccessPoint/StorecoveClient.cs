using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BePeppolCommerce.Core.AccessPoint;

/// <summary>
/// Settings for <see cref="StorecoveClient"/>. Never commit a real key; see Slice 8.
/// <paramref name="SchemeMap"/> maps a Peppol ICD scheme (for example "0208") to Storecove's own
/// scheme name. The spec's examples use names like "DE:VAT" and "FR:CTC" and refer to an external
/// list for the rest, so the Belgian mapping is not yet confirmed. Unmapped schemes are sent as is.
/// </summary>
public sealed record StorecoveOptions(string ApiKey, int LegalEntityId, Uri? BaseUri = null, IReadOnlyDictionary<string, string>? SchemeMap = null)
{
    public static readonly Uri DefaultBaseUri = new("https://api.storecove.com/api/v2/");
}

/// <summary>
/// Storecove API v2 client, written against the public OpenAPI spec at
/// https://api.storecove.com/api/v2/openapi.json (fetched day 029). Sends the UBL as
/// <c>rawDocumentData</c> with <c>parseStrategy=ubl</c> and fetches received documents with
/// <c>format=original</c>. Tested only against a mocked <see cref="HttpMessageHandler"/> and a fake
/// server on a loopback port (Slice 4); it has never been run against the live service.
/// </summary>
public sealed class StorecoveClient : IPeppolAccessPointClient
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly HttpClient _http;
    private readonly StorecoveOptions _options;
    private readonly Uri _baseUri;

    public StorecoveClient(HttpClient http, StorecoveOptions options)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new ArgumentException("Storecove API key is required.", nameof(options));
        var b = options.BaseUri ?? StorecoveOptions.DefaultBaseUri;
        if (b.Scheme != Uri.UriSchemeHttps && !b.IsLoopback)
            throw new ArgumentException("Base URI must be https (plain http is allowed only for loopback test servers).", nameof(options));
        _baseUri = b.AbsoluteUri.EndsWith('/') ? b : new Uri(b.AbsoluteUri + "/");
    }

    public async Task<AccessPointResult<string>> SendAsync(OutboundDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        var body = new
        {
            legalEntityId = _options.LegalEntityId,
            idempotencyGuid = document.IdempotencyKey?.ToString("D"),
            routing = new
            {
                eIdentifiers = new[] { new { scheme = MapScheme(document.Recipient.Scheme), id = document.Recipient.Id } },
            },
            document = new
            {
                // The spec's documentType enum has "invoice" for both invoices and credit notes.
                documentType = "invoice",
                rawDocumentData = new
                {
                    document = Convert.ToBase64String(Encoding.UTF8.GetBytes(document.UblXml)),
                    parseStrategy = "ubl",
                },
            },
        };

        using var request = NewRequest(HttpMethod.Post, "document_submissions");
        request.Content = JsonContent.Create(body, options: Json);
        return await SendCore(request, async (response, status) =>
        {
            var result = await ReadJson<SubmissionResult>(response, cancellationToken);
            return string.IsNullOrEmpty(result?.Guid)
                ? AccessPointResult<string>.Fail(status, new AccessPointError("client", "Success response had no guid."))
                : AccessPointResult<string>.Ok(result.Guid, status);
        }, cancellationToken);
    }

    public async Task<AccessPointResult<InboundDocument>> GetInboundAsync(string providerDocumentId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerDocumentId);
        if (!Guid.TryParse(providerDocumentId, out var guid))
            return AccessPointResult<InboundDocument>.Fail(null, new AccessPointError("client", "Document id is not a GUID."));

        using var request = NewRequest(HttpMethod.Get, $"received_documents/{guid:D}/original");
        return await SendCore(request, async (response, status) =>
        {
            var doc = await ReadJson<Transportable>(response, cancellationToken);
            if (doc?.Guid is { } g && !(Guid.TryParse(g, out var got) && got == guid))
                return AccessPointResult<InboundDocument>.Fail(status, new AccessPointError("client", "Response guid does not match the requested document."));
            var xml = DecodeOriginal(doc?.Original);
            return xml is null
                ? AccessPointResult<InboundDocument>.Fail(status, new AccessPointError("client", "Response had no usable 'original' document."))
                : AccessPointResult<InboundDocument>.Ok(new InboundDocument(guid.ToString("D"), xml), status);
        }, cancellationToken);
    }

    // The spec types 'original' as a string blob without naming its encoding. Accept raw XML or
    // base64 of XML; confirm against a real response before relying on either (see app/README.md).
    internal static string? DecodeOriginal(string? original)
    {
        if (string.IsNullOrWhiteSpace(original)) return null;
        var trimmed = original.TrimStart('\uFEFF').TrimStart();
        if (trimmed.StartsWith('<')) return trimmed;
        try
        {
            var text = Encoding.UTF8.GetString(Convert.FromBase64String(trimmed));
            return text.TrimStart('﻿').TrimStart().StartsWith('<') ? text : null;
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private string MapScheme(string scheme) =>
        _options.SchemeMap is { } map && map.TryGetValue(scheme, out var mapped) ? mapped : scheme;

    private HttpRequestMessage NewRequest(HttpMethod method, string relative)
    {
        var request = new HttpRequestMessage(method, new Uri(_baseUri, relative));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return request;
    }

    private async Task<AccessPointResult<T>> SendCore<T>(
        HttpRequestMessage request,
        Func<HttpResponseMessage, int, Task<AccessPointResult<T>>> onSuccess,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await _http.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            return AccessPointResult<T>.Fail(null, new AccessPointError("transport", ex.Message));
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            return AccessPointResult<T>.Fail(null, new AccessPointError("transport", "Timed out: " + ex.Message));
        }

        using (response)
        {
            var status = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await onSuccess(response, status);
                }
                catch (Exception ex) when (ex is JsonException or InvalidOperationException or NotSupportedException)
                {
                    // InvalidOperationException/NotSupportedException: unsupported charset or content type.
                    return AccessPointResult<T>.Fail(status, new AccessPointError("client", "Unreadable response: " + ex.Message));
                }
            }

            return AccessPointResult<T>.Fail(status, await ReadErrors(response, cancellationToken));
        }
    }

    // 422 responses carry an array of ErrorModel {source, details}; other codes may have no body.
    private static async Task<AccessPointError[]> ReadErrors(HttpResponseMessage response, CancellationToken ct)
    {
        var fallback = new AccessPointError("http", $"{(int)response.StatusCode} {response.ReasonPhrase}".Trim());
        string text;
        try { text = await response.Content.ReadAsStringAsync(ct); }
        catch (HttpRequestException) { return new[] { fallback }; }
        if (string.IsNullOrWhiteSpace(text)) return new[] { fallback };
        try
        {
            var errors = JsonSerializer.Deserialize<ErrorModel[]>(text, Json);
            if (errors is { Length: > 0 })
                return errors.Select(e => new AccessPointError(e.Source ?? "", e.Details ?? "")).ToArray();
        }
        catch (JsonException) { }
        return new[] { fallback };
    }

    private static async Task<T?> ReadJson<T>(HttpResponseMessage response, CancellationToken ct) =>
        await response.Content.ReadFromJsonAsync<T>(Json, ct);

    private sealed record SubmissionResult(string? Guid);

    private sealed record Transportable(string? Guid, string? Direction, string? Original);

    private sealed record ErrorModel(string? Source, string? Details);
}
