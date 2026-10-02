using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using BePeppolCommerce.Core.AccessPoint;
using BePeppolCommerce.Core.Model;
using BePeppolCommerce.Core.Ubl;
using BePeppolCommerce.Core.Validation;

namespace BePeppolCommerce.Core.Outbound;

public enum OutboundStatus
{
    /// <summary>The invoice could not be built or failed validation. Nothing was sent.</summary>
    ValidationFailed,

    /// <summary>The invoice was valid, but the Access Point refused it or could not be reached.</summary>
    SendFailed,

    /// <summary>The Access Point accepted the invoice. <see cref="OutboundResult.SubmissionId"/> is set.</summary>
    Sent,
}

/// <summary>
/// Outcome of one <see cref="OutboundInvoiceSender.SendAsync"/> call. <see cref="UblXml"/> is the exact
/// document that was validated (and sent, if it got that far); it is null only when building failed.
/// </summary>
public sealed record OutboundResult(
    OutboundStatus Status,
    string? UblXml,
    ValidationResult Validation,
    AccessPointResult<string>? Send)
{
    public string? SubmissionId => Status == OutboundStatus.Sent ? Send?.Value : null;
}

/// <summary>
/// Outbound flow: order, then UBL invoice, then Peppol validation, then send through an Access Point
/// client. The send step is reached only when validation reports no blocking finding.
/// </summary>
public sealed class OutboundInvoiceSender
{
    /// <summary>Rule id used when the order cannot be turned into an invoice at all (for example, no lines).</summary>
    public const string BuildRuleId = "BEPC-BUILD";

    private readonly IPeppolAccessPointClient _accessPoint;

    public OutboundInvoiceSender(IPeppolAccessPointClient accessPoint)
    {
        _accessPoint = accessPoint ?? throw new ArgumentNullException(nameof(accessPoint));
    }

    /// <summary>
    /// Builds, validates and sends the invoice for <paramref name="order"/> to the buyer's Peppol
    /// endpoint. When <paramref name="idempotencyKey"/> is null, a key is derived from the invoice
    /// itself (see <see cref="DeriveIdempotencyKey"/>), so retrying an unchanged invoice reuses the
    /// same key and a corrected invoice gets a new one.
    /// </summary>
    public async Task<OutboundResult> SendAsync(Order order, Guid? idempotencyKey = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(order);

        // The builder throws on an order without lines. That is a data problem, not a bug, so it is
        // checked here and reported like a validation failure; any other builder exception escapes.
        if (order.Lines is not { Count: > 0 })
            return new OutboundResult(OutboundStatus.ValidationFailed, null,
                new ValidationResult([new ValidationFinding("BePeppolCommerce", BuildRuleId, "fatal", "An invoice needs at least one line.", "/")]), null);

        var xml = Serialize(PeppolInvoiceBuilder.Build(order));

        // Validate the exact string that would be sent, not the in-memory tree.
        var validation = PeppolValidator.Validate(xml);
        if (!validation.IsValid)
            return new OutboundResult(OutboundStatus.ValidationFailed, xml, validation, null);

        var recipient = new PeppolParticipant(order.Buyer.EndpointSchemeId, order.Buyer.EndpointId);
        var key = idempotencyKey ?? DeriveIdempotencyKey(order, xml);
        var send = await _accessPoint.SendAsync(new OutboundDocument(xml, recipient, key), cancellationToken);
        return new OutboundResult(send.Success ? OutboundStatus.Sent : OutboundStatus.SendFailed, xml, validation, send);
    }

    /// <summary>
    /// A stable GUID from the seller's endpoint, the invoice number, the recipient and the exact UBL
    /// XML: the first 16 bytes of their SHA-256, with the RFC 4122 version (5) and variant bits set.
    /// Fields are length-prefixed so different inputs cannot run together. The builder is
    /// deterministic, so the same order always gets the same key; any change to the invoice or its
    /// recipient gets a different one. How long Storecove remembers a key, and whether it remembers
    /// keys of rejected submissions, is not documented in the spec and is unverified.
    /// </summary>
    public static Guid DeriveIdempotencyKey(Order order, string ublXml)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(ublXml);
        var input = new StringBuilder();
        foreach (var field in new[]
                 {
                     order.Seller.EndpointSchemeId, order.Seller.EndpointId, order.InvoiceNumber,
                     order.Buyer.EndpointSchemeId, order.Buyer.EndpointId, ublXml,
                 })
            input.Append(field.Length).Append(':').Append(field).Append('|');
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input.ToString()))[..16];
        // Big-endian (RFC 4122) byte order: version in the high nibble of byte 6, variant in byte 8.
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x50);
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);
        return new Guid(bytes, bigEndian: true);
    }

    internal static string Serialize(XDocument invoice) =>
        "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + invoice.Root!.ToString(SaveOptions.DisableFormatting);
}
