using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BePeppolCommerce.Core.AccessPoint;
using BePeppolCommerce.Core.Inbound;
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
}

/// <summary>Response body for a received invoice.</summary>
public sealed record InboundWebhookResponse(string ProviderDocumentId, InboundInvoice Invoice);

/// <summary>
/// Webhook for "a document was received". Storecove's public OpenAPI spec does not define the webhook
/// body (day 029), so this accepts a minimal JSON object carrying the received document's id as
/// <c>guid</c> or <c>document_guid</c> (the property name the spec mentions); the value must be a GUID. The handler fetches the
/// document from the Access Point, parses it and returns the normalized invoice.
/// </summary>
public static class InboundWebhook
{
    public const string Route = "/webhooks/inbound";
    public const string SecretHeader = "X-Webhook-Secret";

    /// <summary>The webhook body only carries an id, so anything above this is refused (413).</summary>
    public const int MaxBodyBytes = 16 * 1024;

    public static async Task<IResult> Handle(HttpContext http, IOptions<InboundWebhookOptions> options, IHostEnvironment environment, ILoggerFactory loggers)
    {
        var log = loggers.CreateLogger(typeof(InboundWebhook));

        var secret = options.Value.Secret;
        if (string.IsNullOrEmpty(secret) && !environment.IsDevelopment())
            return Results.Problem("Webhook:Secret is not configured.", statusCode: StatusCodes.Status503ServiceUnavailable);
        if (!string.IsNullOrEmpty(secret) && !SecretMatches(http.Request.Headers[SecretHeader].ToString(), secret))
            return Results.Problem("Missing or wrong webhook secret.", statusCode: StatusCodes.Status401Unauthorized);

        if (http.Request.ContentLength > MaxBodyBytes)
            return Results.Problem($"Body larger than {MaxBodyBytes} bytes.", statusCode: StatusCodes.Status413PayloadTooLarge);

        var body = await ReadBounded(http.Request.Body, http.RequestAborted);
        if (body is null)
            return Results.Problem($"Body larger than {MaxBodyBytes} bytes.", statusCode: StatusCodes.Status413PayloadTooLarge);

        var documentId = DocumentId(body);
        if (documentId is null)
            return Results.Problem("Body must be a JSON object with a 'guid' or 'document_guid' string holding a GUID.", statusCode: StatusCodes.Status400BadRequest);

        if (http.RequestServices.GetService<IPeppolAccessPointClient>() is not { } client)
            return Results.Problem("No Access Point provider is configured (set Storecove:ApiKey).", statusCode: StatusCodes.Status503ServiceUnavailable);

        var fetched = await client.GetInboundAsync(documentId, http.RequestAborted);
        if (!fetched.Success)
        {
            log.LogWarning("Fetching inbound document {DocumentId} failed (HTTP {Status}): {Errors}",
                documentId, fetched.HttpStatus, string.Join("; ", fetched.Errors.Select(e => $"{e.Source}: {e.Details}")));
            return Results.Problem("Could not fetch the document from the Access Point.", statusCode: StatusCodes.Status502BadGateway);
        }

        var parsed = InboundInvoiceParser.Parse(fetched.Value!.UblXml);
        if (!parsed.Success)
        {
            log.LogWarning("Inbound document {DocumentId} could not be parsed: {Error}", documentId, parsed.Error);
            return Results.Problem(parsed.Error, statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return Results.Ok(new InboundWebhookResponse(fetched.Value.ProviderDocumentId, parsed.Invoice!));
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

    // Returns the id in canonical GUID form, so nothing caller-supplied beyond a GUID reaches the
    // provider, the logs or the response.
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
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
