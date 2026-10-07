using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using BePeppolCommerce.Core.AccessPoint;
using BePeppolCommerce.Core.Inbound;
using BePeppolCommerce.Core.Integration;
using Microsoft.Extensions.Options;

namespace BePeppolCommerce.Api;

/// <summary>Configuration section "Webhook".</summary>
public sealed class InboundWebhookOptions
{
    /// <summary>
    /// Shared secret that requests must carry in the <c>X-Webhook-Secret</c> header. Outside the
    /// Development environment the webhook answers 503 until it is set, because the response exposes
    /// the received invoice's parties and amounts. In Development an unset secret means no check.
    /// This is this project's own convention: Storecove's public spec does not describe how its
    /// webhooks authenticate, so match this to the provider's real mechanism before production.
    /// </summary>
    public string? Secret { get; set; }

    /// <summary>
    /// Recommand's webhook signing secret. When set, a request whose <c>X-Signature</c> header is
    /// <c>sha256=&lt;hex&gt;</c>, the HMAC-SHA256 of the raw body under this secret, is accepted without
    /// <see cref="Secret"/>. That header format is the one Recommand's API documents for signed deliveries
    /// (api/webhooks/shared.ts in https://github.com/brbxai/recommand-peppol). Recommand's delivery body
    /// (the event envelope) is still not parsed: the body must carry a document id as described on
    /// <see cref="InboundWebhook"/>. Either this or <see cref="Secret"/> satisfies the 503 rule outside Development.
    /// </summary>
    public string? SigningSecret { get; set; }
}

/// <summary>Response body for a received invoice.</summary>
public sealed record InboundWebhookResponse(string ProviderDocumentId, InboundInvoice Invoice);

/// <summary>
/// Webhook for "a document was received". Storecove's public OpenAPI spec does not define the webhook
/// body (day 029), so this accepts a minimal JSON object carrying the received document's id as
/// <c>guid</c> or <c>document_guid</c> (the property name the spec mentions), whose value must be a
/// GUID, or as <c>documentId</c>, whose value is a GUID or a Recommand-style id (letters, digits,
/// '_' and '-', for example "doc_01JQ..."). All three names are this project's convention: neither
/// provider's real webhook delivery is parsed yet. The handler fetches the document from the Access
/// Point, parses it and returns the normalized invoice.
/// </summary>
public static partial class InboundWebhook
{
    // \z, not $: $ would also match before a trailing newline.
    [GeneratedRegex(@"^[A-Za-z0-9_-]{1,128}\z")]
    private static partial Regex ProviderIdPattern();

    public const string Route = "/webhooks/inbound";
    public const string SecretHeader = "X-Webhook-Secret";
    public const string SignatureHeader = "X-Signature";

    /// <summary>The webhook body only carries an id, so anything above this is refused (413).</summary>
    public const int MaxBodyBytes = 16 * 1024;

    public static async Task<IResult> Handle(HttpContext http, IOptions<InboundWebhookOptions> options, IHostEnvironment environment, ILoggerFactory loggers)
    {
        var log = loggers.CreateLogger(typeof(InboundWebhook));

        var secret = options.Value.Secret;
        var signingSecret = options.Value.SigningSecret;
        var anyAuth = !string.IsNullOrEmpty(secret) || !string.IsNullOrEmpty(signingSecret);
        if (!anyAuth && !environment.IsDevelopment())
            return Results.Problem("Neither Webhook:Secret nor Webhook:SigningSecret is configured.", statusCode: StatusCodes.Status503ServiceUnavailable);

        // The shared-secret header is checked before the body is read. A signature needs the body, so it is
        // checked after the bounded read below; without a signing secret a wrong header is refused here.
        var authenticated = !anyAuth
            || (!string.IsNullOrEmpty(secret) && SecretMatches(http.Request.Headers[SecretHeader].ToString(), secret));
        if (!authenticated && string.IsNullOrEmpty(signingSecret))
            return Unauthorized(log);

        // A body without a usable document id is logged but not recorded as a failed document: there is no
        // document to record, and an anonymous caller could otherwise fill the failure log.
        if (http.Request.ContentLength > MaxBodyBytes)
        {
            log.LogWarning(PeppolLogEvents.InboundPayloadRejected, "Inbound webhook body rejected: larger than {MaxBytes} bytes.", MaxBodyBytes);
            return Results.Problem($"Body larger than {MaxBodyBytes} bytes.", statusCode: StatusCodes.Status413PayloadTooLarge);
        }

        var body = await ReadBounded(http.Request.Body, http.RequestAborted);
        if (body is null)
        {
            log.LogWarning(PeppolLogEvents.InboundPayloadRejected, "Inbound webhook body rejected: larger than {MaxBytes} bytes.", MaxBodyBytes);
            return Results.Problem($"Body larger than {MaxBodyBytes} bytes.", statusCode: StatusCodes.Status413PayloadTooLarge);
        }

        if (!authenticated && !SignatureMatches(http.Request.Headers[SignatureHeader].ToString(), body, signingSecret!))
            return Unauthorized(log);

        var documentId = DocumentId(body);
        if (documentId is null)
        {
            log.LogWarning(PeppolLogEvents.InboundPayloadRejected, "Inbound webhook body rejected: no usable document id.");
            return Results.Problem("Body must be a JSON object with a 'guid' or 'document_guid' string holding a GUID, or a 'documentId' string.", statusCode: StatusCodes.Status400BadRequest);
        }

        if (http.RequestServices.GetService<IPeppolAccessPointClient>() is not { } client)
            return Results.Problem("No Access Point provider is configured (set Storecove:ApiKey or Recommand:ApiKey).", statusCode: StatusCodes.Status503ServiceUnavailable);

        var failures = http.RequestServices.GetService<IFailedDocumentLog>();
        var time = http.RequestServices.GetService<TimeProvider>() ?? TimeProvider.System;

        var fetched = await client.GetInboundAsync(documentId, http.RequestAborted);
        if (!fetched.Success)
        {
            var details = fetched.Errors.Select(e => $"{e.Source}: {e.Details}").ToArray();
            // Same rule as the outbound dispatcher: no status, 408, 429 and 5xx (provider unavailable) and
            // 401, 403, 404 (our key or account id) may succeed later without a change to the document.
            var retryable = fetched.HttpStatus is null or 401 or 403 or 404 or 408 or 429 or >= 500;
            log.LogWarning(PeppolLogEvents.InboundFetchFailed, "Fetching inbound document {DocumentId} failed (HTTP {Status}, retryable: {Retryable}): {Errors}",
                documentId, fetched.HttpStatus, retryable, string.Join("; ", details));
            await PeppolLogEvents.TryRecordAsync(failures,
                new FailedDocument(time.GetUtcNow(), DocumentDirection.Inbound, documentId, "Fetch from Access Point failed", retryable, details),
                log, CancellationToken.None);
            return Results.Problem("Could not fetch the document from the Access Point.", statusCode: StatusCodes.Status502BadGateway);
        }

        var parsed = InboundInvoiceParser.Parse(fetched.Value!.UblXml);
        if (!parsed.Success)
        {
            log.LogWarning(PeppolLogEvents.InboundParseFailed, "Inbound document {DocumentId} could not be parsed: {Error}", documentId, parsed.Error);
            await PeppolLogEvents.TryRecordAsync(failures,
                new FailedDocument(time.GetUtcNow(), DocumentDirection.Inbound, documentId, "Unparseable document", false, [parsed.Error!]),
                log, CancellationToken.None);
            return Results.Problem(parsed.Error, statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return Results.Ok(new InboundWebhookResponse(fetched.Value.ProviderDocumentId, parsed.Invoice!));
    }

    private static IResult Unauthorized(ILogger log)
    {
        log.LogWarning(PeppolLogEvents.InboundUnauthorized, "Inbound webhook call refused: missing or wrong secret or signature.");
        return Results.Problem("Missing or wrong webhook secret or signature.", statusCode: StatusCodes.Status401Unauthorized);
    }

    // "sha256=" followed by the lowercase or uppercase hex HMAC-SHA256 of the raw body.
    internal static bool SignatureMatches(string header, byte[] body, string signingSecret)
    {
        const string prefix = "sha256=";
        if (!header.StartsWith(prefix, StringComparison.Ordinal)) return false;
        byte[] provided;
        try { provided = Convert.FromHexString(header.AsSpan(prefix.Length)); }
        catch (FormatException) { return false; }
        var expected = HMACSHA256.HashData(Encoding.UTF8.GetBytes(signingSecret), body);
        return CryptographicOperations.FixedTimeEquals(provided, expected);
    }

    private static bool SecretMatches(string provided, string expected) =>
        CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(provided)),
            SHA256.HashData(Encoding.UTF8.GetBytes(expected)));

    // Reads at most MaxBodyBytes; returns null if the body is longer (Content-Length can be absent).
    private static async Task<byte[]?> ReadBounded(Stream body, CancellationToken cancellationToken)
    {
        var buffer = new byte[MaxBodyBytes + 1];
        var total = 0;
        int read;
        while (total < buffer.Length && (read = await body.ReadAsync(buffer.AsMemory(total), cancellationToken)) > 0)
            total += read;
        return total > MaxBodyBytes ? null : buffer[..total];
    }

    // Returns a GUID in canonical form, or a Recommand-style id checked against a strict pattern, so
    // nothing caller-supplied beyond those reaches the provider, the logs or the response.
    private static string? DocumentId(byte[] body)
    {
        try
        {
            using var json = JsonDocument.Parse(body);
            if (json.RootElement.ValueKind != JsonValueKind.Object) return null;
            foreach (var name in new[] { "guid", "document_guid" })
                if (json.RootElement.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
                    && Guid.TryParse(value.GetString(), out var guid))
                    return guid.ToString("D");
            if (json.RootElement.TryGetProperty("documentId", out var id) && id.ValueKind == JsonValueKind.String)
            {
                var text = id.GetString()!;
                if (Guid.TryParse(text, out var g)) return g.ToString("D");
                if (ProviderIdPattern().IsMatch(text)) return text;
            }
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
