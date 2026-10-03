using System.Xml.Linq;
using BePeppolCommerce.Core.AccessPoint;
using BePeppolCommerce.Core.Inbound;
using BePeppolCommerce.Core.Model;
using BePeppolCommerce.Core.Ubl;
using BePeppolCommerce.Core.Validation;

namespace BePeppolCommerce.Core.Tests;

public class InboundInvoiceParserTests
{
    private static readonly XNamespace Cac = PeppolInvoiceBuilder.Cac;
    private static readonly XNamespace Cbc = PeppolInvoiceBuilder.Cbc;

    private static XDocument SampleInvoice() =>
        PeppolInvoiceBuilder.Build(OrderJson.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample-order.json"))));

    [Fact]
    public void Parse_SampleInvoice_ExtractsHeaderFields()
    {
        var result = InboundInvoiceParser.Parse(SampleInvoice().ToString());

        Assert.True(result.Success, result.Error);
        var invoice = result.Invoice!;
        Assert.Equal("INV-2026-0001", invoice.InvoiceNumber);
        Assert.Equal(new DateOnly(2026, 9, 28), invoice.IssueDate);
        Assert.Equal("EUR", invoice.Currency);
        Assert.Equal("Example Seller BV (fictitious)", invoice.Seller.Name);
        Assert.Equal(new PeppolParticipant("0208", "0000000097"), invoice.Seller.Endpoint);
        Assert.Equal("Example Buyer NV (fictitious)", invoice.Buyer.Name);
        Assert.Equal(new PeppolParticipant("0208", "0000000196"), invoice.Buyer.Endpoint);
        Assert.Equal(3, invoice.LineCount);
        // 10 x 12.50 + 2 x 45.00 at 21% (260.15) plus 1 x 8.00 at 6% (8.48).
        Assert.Equal(268.63m, invoice.PayableAmount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not xml")]
    [InlineData("<Invoice xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:Invoice-2\">")]
    public void Parse_MalformedXml_ReturnsFailure(string xml)
    {
        var result = InboundInvoiceParser.Parse(xml);

        Assert.False(result.Success);
        Assert.Null(result.Invoice);
        Assert.StartsWith("Not well-formed", result.Error);
    }

    [Fact]
    public void Parse_CreditNoteOrOtherRoot_ReturnsFailure()
    {
        var result = InboundInvoiceParser.Parse(
            "<CreditNote xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2\"><ID xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2\">1</ID></CreditNote>");

        Assert.False(result.Success);
        Assert.Contains("Root element must be a UBL 2.1 Invoice", result.Error);
    }

    [Fact]
    public void Parse_ExternalEntityPayload_IsRejectedWithoutResolving()
    {
        const string xxe = """
            <?xml version="1.0"?>
            <!DOCTYPE Invoice [ <!ENTITY xxe SYSTEM "file:///etc/passwd"> ]>
            <Invoice xmlns="urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"><ID>&xxe;</ID></Invoice>
            """;

        var result = InboundInvoiceParser.Parse(xxe);

        Assert.False(result.Success);
        Assert.StartsWith("Not well-formed or not allowed XML", result.Error);
    }

    [Fact]
    public void Parse_DocumentOverCharacterCap_ReturnsFailure()
    {
        var doc = SampleInvoice();
        doc.Root!.Add(new XElement(Cbc + "Note", new string('x', (int)PeppolValidator.MaxDocumentCharacters)));

        var result = InboundInvoiceParser.Parse(doc.ToString());

        Assert.False(result.Success);
        Assert.StartsWith("Not well-formed or not allowed XML", result.Error);
    }

    [Fact]
    public void Parse_MissingPayableAmount_ReturnsFailure()
    {
        var doc = SampleInvoice();
        doc.Root!.Element(Cac + "LegalMonetaryTotal")!.Element(Cbc + "PayableAmount")!.Remove();

        var result = InboundInvoiceParser.Parse(doc.ToString());

        Assert.False(result.Success);
        Assert.Contains("PayableAmount", result.Error);
    }

    [Theory]
    [InlineData("2026-13-01")]
    [InlineData("28/09/2026")]
    [InlineData("")]
    public void Parse_BadIssueDate_ReturnsFailure(string date)
    {
        var doc = SampleInvoice();
        doc.Root!.Element(Cbc + "IssueDate")!.Value = date;

        var result = InboundInvoiceParser.Parse(doc.ToString());

        Assert.False(result.Success);
        Assert.Contains("IssueDate", result.Error);
    }

    [Fact]
    public void Parse_NoPartyName_FallsBackToRegistrationName_AndNoEndpointGivesNull()
    {
        var doc = SampleInvoice();
        var buyer = doc.Root!.Element(Cac + "AccountingCustomerParty")!.Element(Cac + "Party")!;
        buyer.Element(Cac + "PartyName")?.Remove();
        buyer.Element(Cbc + "EndpointID")!.Remove();
        var legalName = buyer.Element(Cac + "PartyLegalEntity")!.Element(Cbc + "RegistrationName")!.Value;

        var result = InboundInvoiceParser.Parse(doc.ToString());

        Assert.True(result.Success, result.Error);
        Assert.Equal(legalName, result.Invoice!.Buyer.Name);
        Assert.Null(result.Invoice.Buyer.Endpoint);
    }

    [Fact]
    public void Parse_PartyNamePresent_IsPreferredOverRegistrationName()
    {
        var doc = SampleInvoice();
        var seller = doc.Root!.Element(Cac + "AccountingSupplierParty")!.Element(Cac + "Party")!;
        seller.Element(Cbc + "EndpointID")!.AddAfterSelf(new XElement(Cac + "PartyName", new XElement(Cbc + "Name", "  Seller Trading Name  ")));

        var result = InboundInvoiceParser.Parse(doc.ToString());

        Assert.True(result.Success, result.Error);
        Assert.Equal("Seller Trading Name", result.Invoice!.Seller.Name);
    }

    [Fact]
    public void Parse_NoSellerName_ReturnsFailure()
    {
        var doc = SampleInvoice();
        var seller = doc.Root!.Element(Cac + "AccountingSupplierParty")!.Element(Cac + "Party")!;
        seller.Element(Cac + "PartyName")?.Remove();
        seller.Element(Cac + "PartyLegalEntity")!.Element(Cbc + "RegistrationName")!.Remove();

        var result = InboundInvoiceParser.Parse(doc.ToString());

        Assert.False(result.Success);
        Assert.Contains("seller", result.Error);
    }
}
