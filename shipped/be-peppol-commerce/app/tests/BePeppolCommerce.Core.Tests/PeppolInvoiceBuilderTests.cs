using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using BePeppolCommerce.Core.Model;
using BePeppolCommerce.Core.Ubl;

namespace BePeppolCommerce.Core.Tests;

public class PeppolInvoiceBuilderTests
{
    private static readonly XNamespace Cac = PeppolInvoiceBuilder.Cac;
    private static readonly XNamespace Cbc = PeppolInvoiceBuilder.Cbc;

    private static Order LoadSample() =>
        OrderJson.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample-order.json")));

    [Fact]
    public void Build_FromSampleFixture_ProducesWellFormedXmlThatRoundTrips()
    {
        var xml = PeppolInvoiceBuilder.Build(LoadSample()).ToString();

        // Re-parse with a strict reader: throws if not well-formed.
        using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit });
        var reparsed = XDocument.Load(reader);

        Assert.Equal(PeppolInvoiceBuilder.Inv + "Invoice", reparsed.Root!.Name);
    }

    [Fact]
    public void Build_SetsPeppolBis3IdentifiersAndHeader()
    {
        var root = PeppolInvoiceBuilder.Build(LoadSample()).Root!;

        Assert.Equal(PeppolInvoiceBuilder.CustomizationId, root.Element(Cbc + "CustomizationID")!.Value);
        Assert.Equal(PeppolInvoiceBuilder.ProfileId, root.Element(Cbc + "ProfileID")!.Value);
        Assert.Equal("INV-2026-0001", root.Element(Cbc + "ID")!.Value);
        Assert.Equal("2026-09-28", root.Element(Cbc + "IssueDate")!.Value);
        Assert.Equal("2026-10-28", root.Element(Cbc + "DueDate")!.Value);
        Assert.Equal("380", root.Element(Cbc + "InvoiceTypeCode")!.Value);
        Assert.Equal("EUR", root.Element(Cbc + "DocumentCurrencyCode")!.Value);
        Assert.Equal("PO-12345", root.Element(Cbc + "BuyerReference")!.Value);
    }

    [Fact]
    public void Build_EmitsHeaderElementsInUblSchemaOrder()
    {
        var names = PeppolInvoiceBuilder.Build(LoadSample()).Root!.Elements().Select(e => e.Name.LocalName).ToList();

        string[] expected =
        [
            "CustomizationID", "ProfileID", "ID", "IssueDate", "DueDate", "InvoiceTypeCode",
            "DocumentCurrencyCode", "BuyerReference", "AccountingSupplierParty", "AccountingCustomerParty",
            "TaxTotal", "LegalMonetaryTotal", "InvoiceLine", "InvoiceLine", "InvoiceLine",
        ];
        Assert.Equal(expected, names);
    }

    [Fact]
    public void Build_MapsPartiesWithBelgianEndpointScheme()
    {
        var root = PeppolInvoiceBuilder.Build(LoadSample()).Root!;
        var seller = root.Element(Cac + "AccountingSupplierParty")!.Element(Cac + "Party")!;
        var endpoint = seller.Element(Cbc + "EndpointID")!;

        Assert.Equal("0208", endpoint.Attribute("schemeID")!.Value);
        Assert.Equal("0000000097", endpoint.Value);
        Assert.Equal("BE0000000097", seller.Element(Cac + "PartyTaxScheme")!.Element(Cbc + "CompanyID")!.Value);
        Assert.Equal("BE", seller.Descendants(Cbc + "IdentificationCode").Single().Value);
        Assert.Equal("Example Buyer NV (fictitious)",
            root.Element(Cac + "AccountingCustomerParty")!.Descendants(Cbc + "RegistrationName").Single().Value);
    }

    [Fact]
    public void Build_ComputesTotalsAndVatBreakdownPerRate()
    {
        var root = PeppolInvoiceBuilder.Build(LoadSample()).Root!;
        var totals = root.Element(Cac + "LegalMonetaryTotal")!;

        // Lines: 10 x 12.50 = 125.00; 2 x 45.00 = 90.00; 1 x 8.00 = 8.00 → 223.00 net.
        // VAT: 21% of 215.00 = 45.15; 6% of 8.00 = 0.48 → 45.63. Payable 268.63.
        Assert.Equal("223.00", totals.Element(Cbc + "LineExtensionAmount")!.Value);
        Assert.Equal("223.00", totals.Element(Cbc + "TaxExclusiveAmount")!.Value);
        Assert.Equal("268.63", totals.Element(Cbc + "TaxInclusiveAmount")!.Value);
        Assert.Equal("268.63", totals.Element(Cbc + "PayableAmount")!.Value);

        var taxTotal = root.Element(Cac + "TaxTotal")!;
        Assert.Equal("45.63", taxTotal.Element(Cbc + "TaxAmount")!.Value);
        var subtotals = taxTotal.Elements(Cac + "TaxSubtotal")
            .ToDictionary(s => s.Descendants(Cbc + "Percent").Single().Value, s => s.Element(Cbc + "TaxAmount")!.Value);
        Assert.Equal("45.15", subtotals["21"]);
        Assert.Equal("0.48", subtotals["6"]);

        Assert.All(root.Descendants().Where(e => e.Attribute("currencyID") is not null),
            e => Assert.Equal("EUR", e.Attribute("currencyID")!.Value));
    }

    [Fact]
    public void Build_MapsLinesWithUnitCodesAndPrices()
    {
        var lines = PeppolInvoiceBuilder.Build(LoadSample()).Root!.Elements(Cac + "InvoiceLine").ToList();

        Assert.Equal(3, lines.Count);
        var service = lines[1];
        Assert.Equal("HUR", service.Element(Cbc + "InvoicedQuantity")!.Attribute("unitCode")!.Value);
        Assert.Equal("2", service.Element(Cbc + "InvoicedQuantity")!.Value);
        Assert.Equal("90.00", service.Element(Cbc + "LineExtensionAmount")!.Value);
        Assert.Equal("Installation service", service.Descendants(Cbc + "Name").Single().Value);
        Assert.Equal("45", service.Descendants(Cbc + "PriceAmount").Single().Value);
        Assert.Equal("C62", lines[0].Element(Cbc + "InvoicedQuantity")!.Attribute("unitCode")!.Value);
    }

    [Fact]
    public void Build_KeepsUnitPricePrecisionBeyondTwoDecimals()
    {
        var sample = LoadSample();
        var order = sample with { Lines = [sample.Lines[0] with { Quantity = 1000, UnitPrice = 0.125m }] };
        var line = PeppolInvoiceBuilder.Build(order).Root!.Element(Cac + "InvoiceLine")!;

        Assert.Equal("0.125", line.Descendants(Cbc + "PriceAmount").Single().Value);
        Assert.Equal("125.00", line.Element(Cbc + "LineExtensionAmount")!.Value);
    }

    // Null strings are reported by member path; null objects or lines by the structural check.
    [Theory]
    [InlineData("invoiceNumber", "invoiceNumber")]
    [InlineData("currencyCode", "currencyCode")]
    [InlineData("seller.name", "seller.name")]
    [InlineData("seller.endpointId", "seller.endpointId")]
    [InlineData("buyer.endpointSchemeId", "buyer.endpointSchemeId")]
    [InlineData("buyer.address.countryCode", "buyer.address.countryCode")]
    [InlineData("lines.1.description", "lines[1].description")]
    [InlineData("lines.2.id", "lines[2].id")]
    [InlineData("lines.0.vatCategory", "lines[0].vatCategory")]
    [InlineData("seller.address", "missing seller, buyer, an address, or lines")]
    [InlineData("lines.0", "missing seller, buyer, an address, or lines")]
    public void OrderJson_ExplicitNullForNestedRequiredMember_Throws(string path, string expectedInMessage)
    {
        var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample-order.json"));
        System.Text.Json.Nodes.JsonNode node = System.Text.Json.Nodes.JsonNode.Parse(json)!;
        var parts = path.Split('.');
        foreach (var part in parts[..^1])
            node = int.TryParse(part, out var i) ? node[i]! : node[part]!;
        if (int.TryParse(parts[^1], out var index)) node.AsArray()[index] = null;
        else node.AsObject()[parts[^1]] = null;

        var ex = Assert.ThrowsAny<JsonException>(() => OrderJson.Parse(node.Root.ToJsonString()));
        Assert.Contains(expectedInMessage, ex.Message);
    }

    [Fact]
    public void OrderJson_EmptyRequiredString_Throws()
    {
        var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample-order.json"))
            .Replace("\"INV-2026-0001\"", "\"  \"");

        var ex = Assert.ThrowsAny<JsonException>(() => OrderJson.Parse(json));
        Assert.Contains("invoiceNumber", ex.Message);
    }

    [Theory]
    [InlineData("lines")]
    [InlineData("seller")]
    [InlineData("buyer")]
    public void OrderJson_ExplicitNullForRequiredMember_Throws(string member)
    {
        var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample-order.json"));
        var doc = System.Text.Json.Nodes.JsonNode.Parse(json)!.AsObject();
        doc[member] = null;

        Assert.ThrowsAny<JsonException>(() => OrderJson.Parse(doc.ToJsonString()));
    }

    // Note: this output fails Peppol R003 and BR-CO-25 (no buyer reference, no due date or payment
    // terms). That is intended: PeppolValidatorTests checks the validator reports both.
    [Fact]
    public void Build_OmitsOptionalElementsWhenAbsent()
    {
        var order = LoadSample() with { DueDate = null, BuyerReference = null };
        var root = PeppolInvoiceBuilder.Build(order).Root!;

        Assert.Null(root.Element(Cbc + "DueDate"));
        Assert.Null(root.Element(Cbc + "BuyerReference"));
    }

    [Fact]
    public void Build_WithNoLines_Throws()
    {
        var order = LoadSample() with { Lines = [] };
        Assert.Throws<ArgumentException>(() => PeppolInvoiceBuilder.Build(order));
    }

    [Fact]
    public void Build_UsesInvariantCultureForNumbers()
    {
        var previous = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("nl-BE");
            var root = PeppolInvoiceBuilder.Build(LoadSample()).Root!;
            Assert.Equal("268.63", root.Element(Cac + "LegalMonetaryTotal")!.Element(Cbc + "PayableAmount")!.Value);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = previous;
        }
    }

    [Fact]
    public void OrderJson_MalformedInput_Throws()
    {
        Assert.ThrowsAny<JsonException>(() => OrderJson.Parse("{ not json"));
    }
}
