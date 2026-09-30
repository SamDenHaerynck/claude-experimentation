using System.Xml;
using System.Xml.Linq;
using net.sf.saxon.s9api;
using JStringReader = java.io.StringReader;
using StreamSource = javax.xml.transform.stream.StreamSource;

namespace BePeppolCommerce.Core.Validation;

/// <summary>One failed Schematron assert, taken from the SVRL report.</summary>
/// <param name="RuleSet">Which rule set raised it: "CEN-EN16931-UBL" or "PEPPOL-EN16931-UBL".</param>
/// <param name="RuleId">The assert id, for example "BR-CO-16" or "PEPPOL-EN16931-R003".</param>
/// <param name="Flag">The assert flag as written in the rules: "fatal" or "warning".</param>
/// <param name="Message">The assert text.</param>
/// <param name="Location">XPath of the node the assert failed on.</param>
public sealed record ValidationFinding(string RuleSet, string RuleId, string Flag, string Message, string Location)
{
    /// <summary>Only "warning" is non-blocking. Any other or missing flag blocks, to fail safe.</summary>
    public bool IsBlocking => !string.Equals(Flag, "warning", StringComparison.OrdinalIgnoreCase);
}

public sealed record ValidationResult(IReadOnlyList<ValidationFinding> Findings)
{
    public bool IsValid => !Findings.Any(f => f.IsBlocking);
    public IEnumerable<ValidationFinding> Errors => Findings.Where(f => f.IsBlocking);
    public IEnumerable<ValidationFinding> Warnings => Findings.Where(f => !f.IsBlocking);
}

/// <summary>
/// Validates a UBL invoice or credit note against the UBL 2.1 XSD (vendored OASIS schemas), then
/// the official EN16931 (CEN 1.3.15) and Peppol BIS Billing 3.0 (3.0.20) Schematron rules, run by
/// Saxon-HE. The rules are the committed XSLT in Validation/Rules/ (see SOURCE.md). Findings from
/// all three are returned together.
/// Thread-safe: the compiled stylesheets are shared, and each call gets its own transformer.
/// </summary>
public static class PeppolValidator
{
    public static readonly string[] RuleSets = ["CEN-EN16931-UBL", "PEPPOL-EN16931-UBL"];

    private static readonly XNamespace Svrl = "http://purl.oclc.org/dsdl/svrl";

    /// <summary>Rule id of this library's own check that the root is a UBL Invoice or CreditNote.</summary>
    public const string RootElementRuleId = "BEPC-ROOT";

    private static readonly XName[] AcceptedRoots =
    [
        XName.Get("Invoice", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"),
        XName.Get("CreditNote", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2"),
    ];

    private static readonly Lazy<(string Name, XsltExecutable Xslt)[]> Rules =
        new(LoadRules, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Parses untrusted XML and validates it. DTDs are rejected (no entity expansion, no external
    /// resolution), so XXE payloads throw <see cref="XmlException"/> before Saxon sees anything.
    /// </summary>
    public static ValidationResult Validate(string xml) => Validate(ParseUntrusted(xml));

    public static ValidationResult Validate(XDocument invoice)
    {
        // The Schematron rules only fire on Invoice/CreditNote roots, so any other document would
        // come back with zero failed asserts. Reject it explicitly instead.
        var root = invoice.Root?.Name;
        if (root is null || !AcceptedRoots.Contains(root))
            return new ValidationResult([new ValidationFinding("BePeppolCommerce", RootElementRuleId, "fatal",
                $"Root element must be a UBL 2.1 Invoice or CreditNote, found '{root?.ToString() ?? "(none)"}'.", "/")]);

        // The document is re-serialised from the in-memory tree, so it carries no DOCTYPE and no
        // entity references for Saxon's parser to act on.
        var text = invoice.ToString(SaveOptions.DisableFormatting);
        var findings = new List<ValidationFinding>(UblSchema.Validate(invoice));
        foreach (var (name, xslt) in Rules.Value)
        {
            var transformer = xslt.load();
            transformer.setSource(new StreamSource(new JStringReader(text)));
            var destination = new XdmDestination();
            transformer.setDestination(destination);
            transformer.transform();
            var report = XDocument.Parse(destination.getXdmNode().toString());
            findings.AddRange(report.Descendants(Svrl + "failed-assert").Select(f => new ValidationFinding(
                name,
                (string?)f.Attribute("id") ?? "",
                (string?)f.Attribute("flag") ?? "",
                f.Element(Svrl + "text")?.Value.Trim() ?? "",
                (string?)f.Attribute("location") ?? "")));
        }
        return new ValidationResult(findings);
    }

    public static XDocument ParseUntrusted(string xml)
    {
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };
        using var reader = XmlReader.Create(new StringReader(xml), settings);
        return XDocument.Load(reader);
    }

    private static (string, XsltExecutable)[] LoadRules()
    {
        var processor = new Processor(false);
        var assembly = typeof(PeppolValidator).Assembly;
        var rules = RuleSets.Select(name =>
        {
            using var stream = assembly.GetManifestResourceStream($"BePeppolCommerce.Rules.{name}.xslt")
                ?? throw new InvalidOperationException($"Embedded rules {name}.xslt not found.");
            using var sr = new StreamReader(stream);
            // A new compiler per stylesheet: XsltCompiler is not thread-safe, XsltExecutable is.
            var source = new StreamSource(new JStringReader(sr.ReadToEnd()), $"urn:bepeppol:rules:{name}.xslt");
            return (name, processor.newXsltCompiler().compile(source));
        }).ToArray();
        return rules;
    }
}
