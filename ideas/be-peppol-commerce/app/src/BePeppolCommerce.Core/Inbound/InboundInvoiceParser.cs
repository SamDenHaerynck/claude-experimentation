using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using BePeppolCommerce.Core.AccessPoint;
using BePeppolCommerce.Core.Ubl;
using BePeppolCommerce.Core.Validation;

namespace BePeppolCommerce.Core.Inbound;

/// <summary>A party on a received invoice. <see cref="Endpoint"/> is null when the document has no EndpointID.</summary>
public sealed record InboundParty(string Name, PeppolParticipant? Endpoint);

/// <summary>
/// The normalized view of a received Peppol BIS invoice that a caller (in production, a Configured
/// Commerce extension) consumes. It carries the header fields only, not the full UBL.
/// </summary>
public sealed record InboundInvoice(
    string InvoiceNumber,
    DateOnly IssueDate,
    string Currency,
    InboundParty Seller,
    InboundParty Buyer,
    int LineCount,
    decimal PayableAmount);

/// <summary>Outcome of <see cref="InboundInvoiceParser.Parse"/>: either an invoice or an error message.</summary>
public sealed record InboundParseResult(bool Success, InboundInvoice? Invoice, string? Error)
{
    public static InboundParseResult Ok(InboundInvoice invoice) => new(true, invoice, null);

    public static InboundParseResult Fail(string error) => new(false, null, error);
}

/// <summary>
/// Extracts an <see cref="InboundInvoice"/> from received UBL XML. Malformed, oversized or DTD-carrying
/// XML, a root other than a UBL 2.1 Invoice, and missing or unreadable required fields come back as a
/// failed result, never as an exception. This does not run the Peppol rules; call
/// <see cref="PeppolValidator"/> for that.
/// </summary>
public static class InboundInvoiceParser
{
    private static readonly XNamespace Cac = PeppolInvoiceBuilder.Cac;
    private static readonly XNamespace Cbc = PeppolInvoiceBuilder.Cbc;
    private static readonly XName InvoiceRoot = PeppolInvoiceBuilder.Inv + "Invoice";

    public static InboundParseResult Parse(string xml)
    {
        ArgumentNullException.ThrowIfNull(xml);

        XDocument doc;
        try
        {
            doc = PeppolValidator.ParseUntrusted(xml);
        }
        catch (XmlException ex)
        {
            return InboundParseResult.Fail($"Not well-formed or not allowed XML: {ex.Message}");
        }

        var root = doc.Root!;
        if (root.Name != InvoiceRoot)
            return InboundParseResult.Fail($"Root element must be a UBL 2.1 Invoice, found '{root.Name}'.");

        var number = Text(root.Element(Cbc + "ID"));
        if (number is null) return InboundParseResult.Fail("Missing cbc:ID (invoice number).");

        if (!DateOnly.TryParseExact(Text(root.Element(Cbc + "IssueDate")), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var issueDate))
            return InboundParseResult.Fail("Missing or invalid cbc:IssueDate (expected yyyy-MM-dd).");

        var currency = Text(root.Element(Cbc + "DocumentCurrencyCode"));
        if (currency is null) return InboundParseResult.Fail("Missing cbc:DocumentCurrencyCode.");

        var seller = Party(root.Element(Cac + "AccountingSupplierParty"));
        if (seller is null) return InboundParseResult.Fail("Missing seller name (AccountingSupplierParty).");

        var buyer = Party(root.Element(Cac + "AccountingCustomerParty"));
        if (buyer is null) return InboundParseResult.Fail("Missing buyer name (AccountingCustomerParty).");

        var payableText = Text(root.Element(Cac + "LegalMonetaryTotal")?.Element(Cbc + "PayableAmount"));
        if (!decimal.TryParse(payableText, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var payable))
            return InboundParseResult.Fail("Missing or invalid LegalMonetaryTotal/cbc:PayableAmount.");

        var lines = root.Elements(Cac + "InvoiceLine").Count();

        return InboundParseResult.Ok(new InboundInvoice(number, issueDate, currency, seller, buyer, lines, payable));
    }

    // Peppol BIS: PartyName/Name is optional (BT-28/BT-45), PartyLegalEntity/RegistrationName is
    // mandatory (BT-27/BT-44), so prefer the trading name and fall back to the legal name.
    private static InboundParty? Party(XElement? accountingParty)
    {
        var party = accountingParty?.Element(Cac + "Party");
        if (party is null) return null;
        var name = Text(party.Element(Cac + "PartyName")?.Element(Cbc + "Name"))
                   ?? Text(party.Element(Cac + "PartyLegalEntity")?.Element(Cbc + "RegistrationName"));
        if (name is null) return null;
        var endpoint = party.Element(Cbc + "EndpointID");
        var scheme = (string?)endpoint?.Attribute("schemeID");
        var id = Text(endpoint);
        return new InboundParty(name, string.IsNullOrWhiteSpace(scheme) || id is null ? null : new PeppolParticipant(scheme.Trim(), id));
    }

    private static string? Text(XElement? element) =>
        element is null || string.IsNullOrWhiteSpace(element.Value) ? null : element.Value.Trim();
}
