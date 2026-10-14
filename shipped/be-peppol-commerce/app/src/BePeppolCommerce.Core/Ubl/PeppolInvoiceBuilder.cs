using System.Globalization;
using System.Xml.Linq;
using BePeppolCommerce.Core.Model;

namespace BePeppolCommerce.Core.Ubl;

/// <summary>
/// Maps an <see cref="Order"/> to a Peppol BIS Billing 3.0 UBL 2.1 Invoice document.
/// Slice 1 scope: structurally well-formed XML with the BIS 3.0 identifiers and core elements in
/// UBL schema order. Not yet validated against the UBL XSD or EN16931/Peppol Schematron (Slice 2a/2b).
/// </summary>
public static class PeppolInvoiceBuilder
{
    public const string CustomizationId =
        "urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0";
    public const string ProfileId = "urn:fdc:peppol.eu:2017:poacc:billing:01:1.0";
    public const string InvoiceTypeCodeCommercial = "380";

    public static readonly XNamespace Inv = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
    public static readonly XNamespace Cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
    public static readonly XNamespace Cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

    public static XDocument Build(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        if (order.Lines.Count == 0)
            throw new ArgumentException("An invoice needs at least one line.", nameof(order));

        var currency = order.CurrencyCode;
        var lineTotal = order.Lines.Sum(l => l.LineExtensionAmount);
        var docAcs = order.AllowancesCharges ?? [];
        var allowanceTotal = docAcs.Where(a => !a.IsCharge).Sum(a => Money(a.Amount));
        var chargeTotal = docAcs.Where(a => a.IsCharge).Sum(a => Money(a.Amount));
        var taxExclusive = lineTotal - allowanceTotal + chargeTotal;
        // One VAT breakdown per category and rate; allowances reduce and charges increase the base.
        var contributions = order.Lines
            .Select(l => (Cat: Category(l), Rate: Rate(l), Amount: l.LineExtensionAmount,
                Code: l.VatExemptionReasonCode, Text: l.VatExemptionReason))
            .Concat(docAcs.Select(a => (Cat: Category(a.VatCategory), Rate: Rate(a.VatCategory, a.VatPercent),
                Amount: a.IsCharge ? Money(a.Amount) : -Money(a.Amount),
                Code: a.VatExemptionReasonCode, Text: a.VatExemptionReason)));
        var vatGroups = contributions
            .GroupBy(c => (VatCategory: c.Cat, VatPercent: c.Rate))
            .Select(g =>
            {
                var taxable = g.Sum(c => c.Amount);
                var tax = Math.Round(taxable * g.Key.VatPercent / 100m, 2, MidpointRounding.AwayFromZero);
                // The exemption reason lives on the VAT breakdown (BG-23), one per category and rate.
                var exempt = g.FirstOrDefault(c => c.Code is not null || c.Text is not null);
                return (g.Key.VatCategory, g.Key.VatPercent, Taxable: taxable, Tax: tax,
                    ReasonCode: exempt.Code, Reason: exempt.Text);
            })
            .ToList();
        var taxTotal = vatGroups.Sum(g => g.Tax);
        var payable = taxExclusive + taxTotal;

        var root = new XElement(Inv + "Invoice",
            new XAttribute(XNamespace.Xmlns + "cac", Cac),
            new XAttribute(XNamespace.Xmlns + "cbc", Cbc),
            new XElement(Cbc + "CustomizationID", CustomizationId),
            new XElement(Cbc + "ProfileID", ProfileId),
            new XElement(Cbc + "ID", order.InvoiceNumber),
            new XElement(Cbc + "IssueDate", Date(order.IssueDate)),
            order.DueDate is { } due ? new XElement(Cbc + "DueDate", Date(due)) : null,
            new XElement(Cbc + "InvoiceTypeCode", InvoiceTypeCodeCommercial),
            new XElement(Cbc + "DocumentCurrencyCode", currency),
            order.BuyerReference is { } br ? new XElement(Cbc + "BuyerReference", br) : null,
            string.IsNullOrWhiteSpace(order.OrderReference) ? null
                : new XElement(Cac + "OrderReference", new XElement(Cbc + "ID", order.OrderReference)),
            PartyElement("AccountingSupplierParty", order.Seller),
            PartyElement("AccountingCustomerParty", order.Buyer),
            order.DeliveryDate is not null || order.DeliveryCountryCode is not null
                ? new XElement(Cac + "Delivery",
                    order.DeliveryDate is { } dd ? new XElement(Cbc + "ActualDeliveryDate", Date(dd)) : null,
                    order.DeliveryCountryCode is { } dc
                        ? new XElement(Cac + "DeliveryLocation",
                            new XElement(Cac + "Address",
                                new XElement(Cac + "Country", new XElement(Cbc + "IdentificationCode", dc))))
                        : null)
                : null,
            order.Payment is { } pay
                ? new XElement(Cac + "PaymentMeans",
                    new XElement(Cbc + "PaymentMeansCode", "30"), // 30 = credit transfer
                    pay.StructuredCommunication is { } ogm ? new XElement(Cbc + "PaymentID", ogm) : null,
                    new XElement(Cac + "PayeeFinancialAccount", new XElement(Cbc + "ID", pay.Iban)))
                : null,
            string.IsNullOrWhiteSpace(order.PaymentTerms) ? null
                : new XElement(Cac + "PaymentTerms", new XElement(Cbc + "Note", order.PaymentTerms)),
            docAcs.Select(a => new XElement(Cac + "AllowanceCharge",
                new XElement(Cbc + "ChargeIndicator", a.IsCharge ? "true" : "false"),
                a.ReasonCode is null ? null : new XElement(Cbc + "AllowanceChargeReasonCode", a.ReasonCode),
                a.Reason is null ? null : new XElement(Cbc + "AllowanceChargeReason", a.Reason),
                Amount("Amount", Money(a.Amount), currency),
                TaxCategory("TaxCategory", Category(a.VatCategory), Rate(a.VatCategory, a.VatPercent)))),
            new XElement(Cac + "TaxTotal",
                Amount("TaxAmount", taxTotal, currency),
                vatGroups.Select(g => new XElement(Cac + "TaxSubtotal",
                    Amount("TaxableAmount", g.Taxable, currency),
                    Amount("TaxAmount", g.Tax, currency),
                    TaxCategory("TaxCategory", g.VatCategory, g.VatPercent, g.ReasonCode, g.Reason)))),
            new XElement(Cac + "LegalMonetaryTotal",
                Amount("LineExtensionAmount", lineTotal, currency),
                Amount("TaxExclusiveAmount", taxExclusive, currency),
                Amount("TaxInclusiveAmount", payable, currency),
                docAcs.Any(a => !a.IsCharge) ? Amount("AllowanceTotalAmount", allowanceTotal, currency) : null,
                docAcs.Any(a => a.IsCharge) ? Amount("ChargeTotalAmount", chargeTotal, currency) : null,
                Amount("PayableAmount", payable, currency)),
            order.Lines.Select(l => new XElement(Cac + "InvoiceLine",
                new XElement(Cbc + "ID", l.Id),
                new XElement(Cbc + "InvoicedQuantity", new XAttribute("unitCode", l.UnitCode), Num(l.Quantity)),
                Amount("LineExtensionAmount", l.LineExtensionAmount, currency),
                new XElement(Cac + "Item",
                    new XElement(Cbc + "Name", l.Description),
                    TaxCategory("ClassifiedTaxCategory", Category(l), Rate(l))),
                new XElement(Cac + "Price",
                    // Unit price keeps its full precision (BT-146 is not limited to 2 decimals).
                    new XElement(Cbc + "PriceAmount", new XAttribute("currencyID", currency), Num(l.UnitPrice))))));

        return new XDocument(new XDeclaration("1.0", "UTF-8", null), root);
    }

    private static XElement PartyElement(string wrapperName, Party party) =>
        new(Cac + wrapperName,
            new XElement(Cac + "Party",
                new XElement(Cbc + "EndpointID", new XAttribute("schemeID", party.EndpointSchemeId), party.EndpointId),
                new XElement(Cac + "PostalAddress",
                    party.Address.Street is { } s ? new XElement(Cbc + "StreetName", s) : null,
                    party.Address.City is { } c ? new XElement(Cbc + "CityName", c) : null,
                    party.Address.PostalCode is { } p ? new XElement(Cbc + "PostalZone", p) : null,
                    new XElement(Cac + "Country",
                        new XElement(Cbc + "IdentificationCode", party.Address.CountryCode))),
                party.VatNumber is { } vat
                    ? new XElement(Cac + "PartyTaxScheme",
                        new XElement(Cbc + "CompanyID", vat),
                        new XElement(Cac + "TaxScheme", new XElement(Cbc + "ID", "VAT")))
                    : null,
                new XElement(Cac + "PartyLegalEntity",
                    new XElement(Cbc + "RegistrationName", party.Name),
                    party.LegalRegistrationId is { } reg ? new XElement(Cbc + "CompanyID", reg) : null)));

    private static XElement TaxCategory(string name, string category, decimal percent,
        string? reasonCode = null, string? reason = null) =>
        new(Cac + name,
            new XElement(Cbc + "ID", category),
            // Category O ("not subject to VAT") must carry no rate (BR-O-05, BR-O-10).
            category == "O" ? null : new XElement(Cbc + "Percent", Num(percent)),
            reasonCode is null ? null : new XElement(Cbc + "TaxExemptionReasonCode", reasonCode),
            reason is null ? null : new XElement(Cbc + "TaxExemptionReason", reason),
            new XElement(Cac + "TaxScheme", new XElement(Cbc + "ID", "VAT")));

    private static string Category(OrderLine line) => Category(line.VatCategory);
    private static string Category(string category) => category.Trim().ToUpperInvariant();

    // Category O carries no VAT, whatever rate the source system left on the line.
    private static decimal Rate(OrderLine line) => Rate(line.VatCategory, line.VatPercent);
    private static decimal Rate(string category, decimal percent) => Category(category) == "O" ? 0m : percent;

    // Amounts are written with two decimals, so sum the rounded values (BR-CO-10/13 compare to the cent).
    private static decimal Money(decimal v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);

    private static XElement Amount(string name, decimal value, string currency) =>
        new(Cbc + name, new XAttribute("currencyID", currency), value.ToString("0.00", CultureInfo.InvariantCulture));

    private static string Num(decimal value) => value.ToString("0.##########", CultureInfo.InvariantCulture);

    private static string Date(DateOnly d) => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}
