using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BePeppolCommerce.Core.AccessPoint;

/// <summary>
/// Settings for <see cref="RecommandClient"/>. Never commit a real key; the API host binds these from
/// configuration (<c>AccessPointConfig</c>) and <see cref="Validate"/> holds the rules.
/// <paramref name="ApiKey"/> and <paramref name="ApiSecret"/> are the pair Recommand's dashboard
/// issues for HTTP Basic auth. <paramref name="CompanyId"/> is Recommand's id for the sending company
/// (for example "c_01JQ...").
/// </summary>
public sealed record RecommandOptions(string ApiKey, string ApiSecret, string CompanyId, Uri? BaseUri = null)
{
    public static readonly Uri DefaultBaseUri = new("https://app.recommand.eu/");

    /// <summary>
    /// Returns one message per invalid setting, each starting with the setting's name and never
    /// containing its value. Empty when the options are usable. Shared by the client constructor and
    /// the API host's startup validation.
    /// </summary>
    public static IReadOnlyList<string> Validate(RecommandOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var problems = new List<string>();
        if (string.IsNullOrWhiteSpace(options.ApiKey)) problems.Add("ApiKey is required.");
        else if (options.ApiKey.Contains(':')) problems.Add("ApiKey must not contain ':' (it is the Basic auth user name).");
        if (string.IsNullOrWhiteSpace(options.ApiSecret)) problems.Add("ApiSecret is required.");
        if (string.IsNullOrWhiteSpace(options.CompanyId) || !RecommandClient.IsValidId(options.CompanyId))
            problems.Add("CompanyId is required and may contain only letters, digits, '_' and '-'.");
        if (options.BaseUri is { } b && !AccessPointHttp.IsAllowedBaseUri(b))
            problems.Add("BaseUri must be https (plain http only for loopback test servers).");
        return problems;
    }

    /// <summary>Hides the key and secret, so options can be logged.</summary>
    public override string ToString() => $"RecommandOptions {{ CompanyId = {CompanyId}, BaseUri = {BaseUri} }}";
}

/// <summary>
/// Recommand Peppol API client, written against the OpenAPI 3.1 spec served at
/// https://peppol.recommand.eu/openapi (fetched day 033). Sends the UBL with
/// <c>POST /api/v1/{companyId}/send</c> and <c>documentType = "xml"</c>, and fetches received
/// documents with <c>GET /api/v1/documents/{documentId}</c>. The send endpoint has no idempotency
/// key, so <see cref="OutboundDocument.IdempotencyKey"/> is not sent and a retried send can deliver
/// twice. Tested only against a mocked <see cref="HttpMessageHandler"/>; it has never been run
/// against the live service.
/// </summary>
public sealed partial class RecommandClient : IPeppolAccessPointClient
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    // Recommand ids look like "doc_01JQZ8X0M4T7RB6K9V2NDHW3PA". Anything outside this set is refused
    // before it reaches a URL path, so an id such as ".." cannot change the request target.
    [GeneratedRegex(@"^[A-Za-z0-9_-]{1,128}\z")]
    private static partial Regex IdPattern();

    internal static bool IsValidId(string id) => IdPattern().IsMatch(id);

    private readonly HttpClient _http;
    private readonly RecommandOptions _options;
    private readonly Uri _baseUri;
    private readonly AuthenticationHeaderValue _auth;

    public RecommandClient(HttpClient http, RecommandOptions options)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        if (RecommandOptions.Validate(options) is { Count: > 0 } problems)
            throw new ArgumentException("Recommand options are invalid: " + string.Join(" ", problems), nameof(options));
        var b = options.BaseUri ?? RecommandOptions.DefaultBaseUri;
        _baseUri = b.AbsoluteUri.EndsWith('/') ? b : new Uri(b.AbsoluteUri + "/");
        _auth = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.ApiKey}:{options.ApiSecret}")));
    }

    public async Task<AccessPointResult<string>> SendAsync(OutboundDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        // doctypeId and processId are left out. The spec says Recommand detects them for raw XML
        // "where supported" and resolves them against the recipient; whether that covers every
        // recipient is unverified, so a "cannot detect" error would mean sending doctypeId here.
        var body = new
        {
            recipient = document.Recipient.ToString(),
            documentType = "xml",
            document = document.UblXml,
        };

        using var request = NewRequest(HttpMethod.Post, $"api/v1/{_options.CompanyId}/send");
        request.Content = JsonContent.Create(body, options: Json);
        return await AccessPointHttp.SendAsync(_http, request, async (response, status) =>
        {
            var result = await response.Content.ReadFromJsonAsync<SendResult>(Json, cancellationToken);
            if (result?.Success != true)
                return AccessPointResult<string>.Fail(status, new AccessPointError("client", "Success response did not report success."));
            // The spec: false means Peppol routing failed or the access point refused it, and the
            // document went by email instead. That is not a Peppol delivery, so it is not reported as
            // sent; with a 2xx status the dispatcher treats it as permanent and does not resend.
            if (result.SentOverPeppol == false)
                return AccessPointResult<string>.Fail(status, new AccessPointError("provider",
                    $"Not sent over Peppol; Recommand delivered it by email instead (document {result.Id})."));
            return string.IsNullOrEmpty(result.Id)
                ? AccessPointResult<string>.Fail(status, new AccessPointError("client", "Success response had no id."))
                : AccessPointResult<string>.Ok(result.Id, status);
        }, ReadErrors, cancellationToken);
    }

    public async Task<AccessPointResult<InboundDocument>> GetInboundAsync(string providerDocumentId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerDocumentId);
        if (!IdPattern().IsMatch(providerDocumentId))
            return AccessPointResult<InboundDocument>.Fail(null, new AccessPointError("client", "Document id has an unexpected format."));

        using var request = NewRequest(HttpMethod.Get, $"api/v1/documents/{providerDocumentId}");
        return await AccessPointHttp.SendAsync(_http, request, async (response, status) =>
        {
            var result = await response.Content.ReadFromJsonAsync<GetResult>(Json, cancellationToken);
            var doc = result?.Document;
            if (result?.Success != true || doc is null)
                return AccessPointResult<InboundDocument>.Fail(status, new AccessPointError("client", "Response had no document."));
            if (doc.Id is { } id && id != providerDocumentId)
                return AccessPointResult<InboundDocument>.Fail(status, new AccessPointError("client", "Response id does not match the requested document."));
            if (doc.CompanyId is { } company && company != _options.CompanyId)
                return AccessPointResult<InboundDocument>.Fail(status, new AccessPointError("client", "Document belongs to another company."));
            if (doc.Direction != "incoming")
                return AccessPointResult<InboundDocument>.Fail(status, new AccessPointError("client", "Document is not an incoming document."));
            var xml = doc.Xml?.TrimStart('﻿').TrimStart();
            return string.IsNullOrEmpty(xml) || !xml.StartsWith('<')
                ? AccessPointResult<InboundDocument>.Fail(status, new AccessPointError("client", "Response had no usable 'xml' document."))
                : AccessPointResult<InboundDocument>.Ok(new InboundDocument(providerDocumentId, xml), status);
        }, ReadErrors, cancellationToken);
    }

    private HttpRequestMessage NewRequest(HttpMethod method, string relative)
    {
        var request = new HttpRequestMessage(method, new Uri(_baseUri, relative));
        request.Headers.Authorization = _auth;
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return request;
    }

    // Error responses (400, 404, 422, 500 in the spec) carry {success: false, errors: {field: [messages]}}.
    private static async Task<AccessPointError[]> ReadErrors(HttpResponseMessage response, CancellationToken ct)
    {
        var fallback = AccessPointHttp.StatusError(response);
        var text = await AccessPointHttp.ReadBodyAsync(response, ct);
        if (text is null) return new[] { fallback };
        try
        {
            var body = JsonSerializer.Deserialize<ErrorBody>(text, Json);
            var errors = body?.Errors?
                .SelectMany(kv => (kv.Value ?? []).Select(m => new AccessPointError(kv.Key, m ?? "")))
                .ToArray();
            if (errors is { Length: > 0 }) return errors;
        }
        catch (JsonException) { }
        return new[] { fallback };
    }

    private sealed record SendResult(bool? Success, string? Id, bool? SentOverPeppol);

    private sealed record GetResult(bool? Success, RecommandDocument? Document);

    private sealed record RecommandDocument(string? Id, string? CompanyId, string? Direction, string? Xml);

    private sealed record ErrorBody(bool? Success, Dictionary<string, string?[]?>? Errors);
}
