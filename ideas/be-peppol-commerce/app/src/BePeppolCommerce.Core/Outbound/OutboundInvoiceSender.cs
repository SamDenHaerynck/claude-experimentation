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
    /// endpoint. When <paramref name="idempotencyKey"/> is null, a key is derived from the seller's
    /// endpoint and the invoice number (see <see cref="DeriveIdempotencyKey"/>), so retrying the same
    /// invoice reuses the same key.
    /// </summary>
    public async Task<OutboundResult> SendAsync(Order order, Guid? idempotencyKey = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(order);

        string xml;
        try
        {
            xml = Serialize(PeppolInvoiceBuilder.Build(order));
        }
        catch (ArgumentException ex)
        {
            return new OutboundResult(OutboundStatus.ValidationFailed, null,
                new ValidationResult([new ValidationFinding("BePeppolCommerce", BuildRuleId, "fatal", ex.Message, "/")]), null);
        }

        // Validate the exact string that would be sent, not the in-memory tree.
        var validation = PeppolValidator.Validate(xml);
        if (!validation.IsValid)
            return new OutboundResult(OutboundStatus.ValidationFailed, xml, validation, null);

        var recipient = new PeppolParticipant(order.Buyer.EndpointSchemeId, order.Buyer.EndpointId);
        var key = idempotencyKey ?? DeriveIdempotencyKey(order);
        var send = await _accessPoint.SendAsync(new OutboundDocument(xml, recipient, key), cancellationToken);
        return new OutboundResult(send.Success ? OutboundStatus.Sent : OutboundStatus.SendFailed, xml, validation, send);
    }

    /// <summary>
    /// A stable GUID from the seller's endpoint scheme and id and the invoice number: the first 16
    /// bytes of their SHA-256. The same invoice from the same seller always gets the same key.
    /// </summary>
    public static Guid DeriveIdempotencyKey(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        var input = $"{order.Seller.EndpointSchemeId}:{order.Seller.EndpointId}|{order.InvoiceNumber}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return new Guid(hash.AsSpan(0, 16));
    }

    internal static string Serialize(XDocument invoice) =>
        "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + invoice.Root!.ToString(SaveOptions.DisableFormatting);
}
