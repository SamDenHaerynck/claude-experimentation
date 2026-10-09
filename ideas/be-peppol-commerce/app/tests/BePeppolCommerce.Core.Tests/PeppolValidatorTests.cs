using System.Xml;
using System.Xml.Linq;
using BePeppolCommerce.Core.Model;
using BePeppolCommerce.Core.Ubl;
using BePeppolCommerce.Core.Validation;

namespace BePeppolCommerce.Core.Tests;

public class PeppolValidatorTests
{
    private static readonly XNamespace Cbc = PeppolInvoiceBuilder.Cbc;
    private static readonly XNamespace Cac = PeppolInvoiceBuilder.Cac;

    private static Order LoadSample() =>
        OrderJson.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample-order.json")));

    private static IEnumerable<string> ErrorIds(ValidationResult r) => r.Errors.Select(e => e.RuleId);

    [Fact]
    public void SampleFixtureInvoice_PassesCenAndPeppolRules()
    {
        var result = PeppolValidator.Validate(PeppolInvoiceBuilder.Build(LoadSample()));

        Assert.True(result.IsValid, string.Join("\n", result.Findings.Select(f => $"{f.RuleSet} {f.RuleId}: {f.Message}")));
        Assert.Empty(result.Findings);
    }

    // Review day 037: only S and Z could produce a valid invoice. One validator-backed case per category.
    private static Order WithCategory(string category, string? reasonCode, bool keepVatNumbers = true)
    {
        var sample = LoadSample();
        return sample with
        {
            Seller = keepVatNumbers ? sample.Seller : sample.Seller with { VatNumber = null },
            Buyer = keepVatNumbers ? sample.Buyer : sample.Buyer with { VatNumber = null },
            Lines = sample.Lines.Select(l => l with { VatCategory = category, VatPercent = 0, VatExemptionReasonCode = reasonCode }).ToList(),
        };
    }

    private static void AssertValid(Order order)
    {
        var result = PeppolValidator.Validate(PeppolInvoiceBuilder.Build(order));
        Assert.True(result.IsValid, string.Join("\n", result.Errors.Select(f => $"{f.RuleSet} {f.RuleId}: {f.Message}")));
    }

    [Fact]
    public void IntraEuSupply_K_WithReasonAndDelivery_IsValid()
    {
        var order = WithCategory("K", "VATEX-EU-IC") with
        {
            DeliveryDate = new DateOnly(2026, 9, 27),
            DeliveryCountryCode = "NL",
        };
        order = order with { Buyer = order.Buyer with { VatNumber = "NL000099998B57", Address = order.Buyer.Address with { CountryCode = "NL" } } };
        AssertValid(order);
    }

    [Fact]
    public void IntraEuSupply_K_WithoutDelivery_FailsBrIc11And12()
    {
        var ids = ErrorIds(PeppolValidator.Validate(PeppolInvoiceBuilder.Build(WithCategory("K", "VATEX-EU-IC")))).ToList();
        Assert.Contains("BR-IC-11", ids);
        Assert.Contains("BR-IC-12", ids);
    }

    [Fact]
    public void ReverseCharge_AE_WithReason_IsValid() => AssertValid(WithCategory("AE", "VATEX-EU-AE"));

    [Fact]
    public void ReverseCharge_AE_WithoutReason_FailsBrAe10() =>
        Assert.Contains("BR-AE-10", ErrorIds(PeppolValidator.Validate(PeppolInvoiceBuilder.Build(WithCategory("AE", null)))));

    [Fact]
    public void Exempt_E_WithReason_IsValid() => AssertValid(WithCategory("E", "VATEX-EU-132"));

    [Fact]
    public void NotSubjectToVat_O_OmitsRateAndIsValid()
    {
        var order = WithCategory("O", "VATEX-EU-O", keepVatNumbers: false);
        AssertValid(order with { Seller = order.Seller with { LegalRegistrationId = "0000000097" } });
    }

    [Fact]
    public void NotSubjectToVat_O_IgnoresLeftoverRate()
    {
        var order = WithCategory("O", "VATEX-EU-O", keepVatNumbers: false);
        AssertValid(order with
        {
            Seller = order.Seller with { LegalRegistrationId = "0000000097" },
            Lines = order.Lines.Select(l => l with { VatPercent = 21 }).ToList(),
        });
    }

    [Fact]
    public void LowercaseCategory_IsNormalised() =>
        AssertValid(LoadSample() with { Lines = LoadSample().Lines.Select(l => l with { VatCategory = " s" }).ToList() });

    [Fact]
    public void BrokenInvoice_FailsWithExpectedRuleIds()
    {
        var doc = PeppolInvoiceBuilder.Build(LoadSample());
        doc.Descendants(Cbc + "PayableAmount").Single().Value = "1.00";
        doc.Descendants(Cbc + "EndpointID").First().SetAttributeValue("schemeID", "9999");

        var result = PeppolValidator.Validate(doc);

        Assert.False(result.IsValid);
        Assert.Contains("BR-CO-16", ErrorIds(result));             // CEN: payable amount = total - prepaid + rounding
        Assert.Contains("BR-CL-25", ErrorIds(result));             // CEN: endpoint scheme code list
        Assert.Contains("PEPPOL-EN16931-CL008", ErrorIds(result)); // Peppol: endpoint scheme code list
        var finding = result.Errors.First(e => e.RuleId == "BR-CO-16");
        Assert.Equal("CEN-EN16931-UBL", finding.RuleSet);
        Assert.Equal("fatal", finding.Flag);
        Assert.False(string.IsNullOrWhiteSpace(finding.Message));
        Assert.False(string.IsNullOrWhiteSpace(finding.Location));
    }

    [Fact]
    public void UnknownElement_FailsUblXsd()
    {
        var doc = PeppolInvoiceBuilder.Build(LoadSample());
        doc.Root!.Element(Cbc + "IssueDate")!.AddBeforeSelf(new XElement(Cbc + "Bogus", "x"));

        var result = PeppolValidator.Validate(doc);

        Assert.False(result.IsValid);
        var xsd = Assert.Single(result.Errors, e => e.RuleSet == "UBL-2.1-XSD");
        Assert.Contains("Bogus", xsd.Message);

        var fromString = PeppolValidator.Validate(doc.ToString());
        Assert.StartsWith("line ", Assert.Single(fromString.Errors, e => e.RuleSet == "UBL-2.1-XSD").Location);
    }

    [Fact]
    public void OrderWithoutBuyerReference_FailsPeppolR003()
    {
        var order = LoadSample() with { BuyerReference = null };

        var result = PeppolValidator.Validate(PeppolInvoiceBuilder.Build(order));

        Assert.False(result.IsValid);
        var r003 = Assert.Single(result.Errors, e => e.RuleId == "PEPPOL-EN16931-R003");
        Assert.Equal("PEPPOL-EN16931-UBL", r003.RuleSet);
    }

    [Fact]
    public void OrderWithoutDueDate_FailsBrCo25()
    {
        // No due date and no payment terms while an amount is due.
        var result = PeppolValidator.Validate(PeppolInvoiceBuilder.Build(LoadSample() with { DueDate = null }));

        Assert.Contains("BR-CO-25", ErrorIds(result));
    }

    [Fact]
    public void ExemptVatCategoryWithoutReason_FailsBrE10()
    {
        // The builder does not emit exemption reasons yet, so category E (exempt) must be caught here.
        var sample = LoadSample();
        var order = sample with { Lines = [sample.Lines[0] with { VatCategory = "E", VatPercent = 0 }] };

        var result = PeppolValidator.Validate(PeppolInvoiceBuilder.Build(order));

        Assert.Contains("BR-E-10", ErrorIds(result));
    }

    [Fact]
    public void ValidateString_RejectsExternalEntityPayload()
    {
        // Classic XXE: a DTD declaring an external entity, referenced from BuyerReference.
        var body = PeppolInvoiceBuilder.Build(LoadSample()).ToString().Replace(">PO-12345<", ">&xxe;<");
        Assert.Contains("&xxe;", body);
        var payload = "<!DOCTYPE Invoice [<!ENTITY xxe SYSTEM \"file:///etc/passwd\">]>\n" + body;

        Assert.Throws<XmlException>(() => PeppolValidator.Validate(payload));
    }

    [Fact]
    public void ValidateXDocument_IgnoresDoctypeWithExternalDtd()
    {
        // An XDocument can carry a DOCTYPE naming an external DTD. It must never be fetched: the
        // path does not exist, so any attempt would throw instead of validating.
        var doc = PeppolInvoiceBuilder.Build(LoadSample());
        doc.AddFirst(new XDocumentType("Invoice", null, "file:///nonexistent-bepeppol/x.dtd", null));

        var result = PeppolValidator.Validate(doc);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateXDocument_DoesNotFetchExternalDtdOverHttp()
    {
        using var listener = new System.Net.HttpListener();
        var port = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties()
            .GetActiveTcpListeners().Select(e => e.Port).DefaultIfEmpty(40000).Max() + 1;
        var prefix = $"http://127.0.0.1:{port}/";
        listener.Prefixes.Add(prefix);
        listener.Start();
        var hits = 0;
        _ = Task.Run(async () =>
        {
            while (listener.IsListening)
            {
                try { var ctx = await listener.GetContextAsync(); Interlocked.Increment(ref hits); ctx.Response.Close(); }
                catch { return; }
            }
        });
        var doc = PeppolInvoiceBuilder.Build(LoadSample());
        doc.AddFirst(new XDocumentType("Invoice", null, prefix + "x.dtd", null));

        PeppolValidator.Validate(doc);

        Assert.Equal(0, hits);
    }

    [Fact]
    public void ValidateString_AcceptsPlainXml()
    {
        var result = PeppolValidator.Validate(PeppolInvoiceBuilder.Build(LoadSample()).ToString());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("<Order xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:Order-2\"/>")]
    [InlineData("<Invoice/>")]
    public void NonInvoiceRoot_IsRejected(string xml)
    {
        var result = PeppolValidator.Validate(xml);

        Assert.False(result.IsValid);
        Assert.Equal(PeppolValidator.RootElementRuleId, Assert.Single(result.Findings).RuleId);
    }

    [Fact]
    public void WarningFlag_IsNotBlocking_AndUnknownFlagIs()
    {
        Assert.False(new ValidationFinding("x", "id", "warning", "", "").IsBlocking);
        Assert.True(new ValidationFinding("x", "id", "fatal", "", "").IsBlocking);
        Assert.True(new ValidationFinding("x", "id", "", "", "").IsBlocking);
    }

    [Fact]
    public async Task ParallelValidation_GivesSameResults()
    {
        var good = PeppolInvoiceBuilder.Build(LoadSample());
        var bad = PeppolInvoiceBuilder.Build(LoadSample() with { BuyerReference = null });

        var results = await Task.WhenAll(Enumerable.Range(0, 8).Select(i =>
            Task.Run(() => PeppolValidator.Validate(i % 2 == 0 ? good : bad))));

        for (var i = 0; i < results.Length; i++)
            Assert.Equal(i % 2 == 0, results[i].IsValid);
    }
}
