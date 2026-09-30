using net.sf.saxon.s9api;
using JFile = java.io.File;
using StreamSource = javax.xml.transform.stream.StreamSource;

namespace BePeppolCommerce.Core.Validation;

/// <summary>
/// Maintainer utility: compiles an ISO Schematron (.sch) file to XSLT 2.0 with the ISO skeleton
/// (iso_dsdl_include.xsl, then iso_abstract_expand.xsl, then iso_svrl_for_xslt2.xsl). Used only by
/// tools/RulesGen to regenerate the committed files in Validation/Rules/. Inputs are trusted local
/// files (the upstream .sch and skeleton), never runtime data.
/// </summary>
public static class SchematronCompiler
{
    private static readonly string[] SkeletonSteps =
        ["iso_dsdl_include.xsl", "iso_abstract_expand.xsl", "iso_svrl_for_xslt2.xsl"];

    public static string CompileToXslt(string skeletonDir, string schPath)
    {
        var processor = new Processor(false);
        var compiler = processor.newXsltCompiler();
        var node = processor.newDocumentBuilder().build(new JFile(schPath));
        foreach (var step in SkeletonSteps)
        {
            var transformer = compiler.compile(new StreamSource(new JFile(Path.Combine(skeletonDir, step)))).load();
            transformer.setSource(node.asSource());
            var destination = new XdmDestination();
            transformer.setDestination(destination);
            transformer.transform();
            node = destination.getXdmNode();
        }
        return node.toString();
    }
}
