using net.sf.saxon.s9api;
using JStringReader = java.io.StringReader;
using Source = javax.xml.transform.Source;
using StreamSource = javax.xml.transform.stream.StreamSource;
using URIResolver = javax.xml.transform.URIResolver;

namespace BePeppolCommerce.Core.Validation;

/// <summary>
/// Compiles an ISO Schematron (.sch) file to an executable XSLT 2.0 stylesheet with the ISO
/// skeleton (iso_dsdl_include.xsl, then iso_abstract_expand.xsl, then iso_svrl_for_xslt2.xsl).
/// The skeleton (MIT, Validation/Skeleton/) and the .sch files (downloaded at build time, see the
/// csproj and Validation/Rules/SOURCE.md) are embedded resources. The Peppol rules may not be
/// redistributed, so no compiled copy is committed; they are compiled in memory on first use.
/// </summary>
internal static class SchematronCompiler
{
    private const string SkeletonPrefix = "BePeppolCommerce.Skeleton.";
    private const string SchPrefix = "BePeppolCommerce.Sch.";
    private const string SkeletonBase = "urn:bepeppol:skeleton/";

    private static readonly string[] SkeletonSteps =
        ["iso_dsdl_include.xsl", "iso_abstract_expand.xsl", "iso_svrl_for_xslt2.xsl"];

    public static XsltExecutable Compile(Processor processor, string ruleSet)
    {
        var node = processor.newDocumentBuilder().build(Source(SchPrefix, ruleSet + ".sch", $"urn:bepeppol:sch/{ruleSet}.sch"));
        foreach (var step in SkeletonSteps)
        {
            var transformer = NewCompiler(processor).compile(SkeletonSource(step)).load();
            transformer.setSource(node.asSource());
            var destination = new XdmDestination();
            transformer.setDestination(destination);
            transformer.transform();
            node = destination.getXdmNode();
        }
        // A new compiler per stylesheet: XsltCompiler is not thread-safe, XsltExecutable is.
        return NewCompiler(processor).compile(node.asSource());
    }

    private static XsltCompiler NewCompiler(Processor processor)
    {
        var compiler = processor.newXsltCompiler();
        compiler.setURIResolver(new SkeletonResolver());
        return compiler;
    }

    private static StreamSource SkeletonSource(string name) => Source(SkeletonPrefix, name, SkeletonBase + name);

    private static StreamSource Source(string prefix, string name, string systemId)
    {
        using var stream = typeof(SchematronCompiler).Assembly.GetManifestResourceStream(prefix + name)
            ?? throw new InvalidOperationException($"Embedded resource {prefix}{name} not found.");
        using var reader = new StreamReader(stream);
        return new StreamSource(new JStringReader(reader.ReadToEnd()), systemId);
    }

    /// <summary>Serves xsl:import/include from the embedded skeleton only; anything else is refused.</summary>
    private sealed class SkeletonResolver : java.lang.Object, URIResolver
    {
        public Source resolve(string href, string @base)
        {
            if (href.Contains('/') || href.Contains('\\') || !href.EndsWith(".xsl", StringComparison.Ordinal))
                throw new javax.xml.transform.TransformerException($"Refusing to resolve '{href}'.");
            return SkeletonSource(href);
        }
    }
}
