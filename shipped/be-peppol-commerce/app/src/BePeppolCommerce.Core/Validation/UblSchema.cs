using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace BePeppolCommerce.Core.Validation;

/// <summary>
/// UBL 2.1 XSD validation for Invoice and CreditNote, using the vendored OASIS schemas in
/// Validation/Schemas/UBL-2.1 (embedded resources). Schema imports resolve only to those embedded
/// files; any other URI is refused, so nothing is fetched from the network or disk.
/// </summary>
internal static class UblSchema
{
    public const string RuleSetName = "UBL-2.1-XSD";
    private const string ResourcePrefix = "BePeppolCommerce.Schemas.UBL-2.1/";
    private static readonly Uri BaseUri = new("http://bepeppol.invalid/UBL-2.1/");

    private static readonly Lazy<XmlSchemaSet> Schemas = new(Load, LazyThreadSafetyMode.ExecutionAndPublication);

    // XmlSchemaSet instance members are not documented as thread-safe, so validation is serialised.
    private static readonly object Gate = new();

    public static IEnumerable<ValidationFinding> Validate(XDocument document)
    {
        var findings = new List<ValidationFinding>();
        var schemas = Schemas.Value;
        lock (Gate)
        {
            document.Validate(schemas, (_, e) => findings.Add(new ValidationFinding(
                RuleSetName,
                "XSD",
                e.Severity == XmlSeverityType.Error ? "fatal" : "warning",
                e.Message,
                e.Exception is { LineNumber: > 0 } ex ? $"line {ex.LineNumber}, position {ex.LinePosition}" : "")));
        }
        return findings;
    }

    private static XmlSchemaSet Load()
    {
        var resolver = new EmbeddedResolver();
        var set = new XmlSchemaSet { XmlResolver = resolver };
        foreach (var main in new[] { "maindoc/UBL-Invoice-2.1.xsd", "maindoc/UBL-CreditNote-2.1.xsd" })
        {
            var uri = new Uri(BaseUri, main);
            using var reader = XmlReader.Create((Stream)resolver.GetEntity(uri, null, typeof(Stream))!, ReaderSettings(resolver), uri.AbsoluteUri);
            set.Add(null, reader);
        }
        set.Compile();
        return set;
    }

    // The OASIS xmldsig schema carries an internal DTD subset (entity shorthands), so DTDs must be
    // parsed here. These are trusted, pinned files, and the resolver serves only embedded resources.
    private static XmlReaderSettings ReaderSettings(XmlResolver resolver) =>
        new() { DtdProcessing = DtdProcessing.Parse, XmlResolver = resolver };

    private sealed class EmbeddedResolver : XmlResolver
    {
        public override object? GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
        {
            var relative = BaseUri.MakeRelativeUri(absoluteUri).OriginalString;
            if (!absoluteUri.AbsoluteUri.StartsWith(BaseUri.AbsoluteUri, StringComparison.Ordinal) || relative.Contains(".."))
                throw new XmlException($"Refusing to resolve '{absoluteUri}': only the embedded UBL 2.1 schemas are allowed.");
            return typeof(UblSchema).Assembly.GetManifestResourceStream(ResourcePrefix + relative)
                ?? throw new XmlException($"Embedded schema '{relative}' not found.");
        }
    }
}
