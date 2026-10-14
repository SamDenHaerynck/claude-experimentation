using JFile = java.io.File;
using net.sf.saxon.s9api;
using javax.xml.transform.stream;

if (args[0]=="xsd") { Xsd.Check(args[1], args[2]); return; }
if (args[0]=="gen") { Gen.Emit(args[1], args[2]); return; }
var proc = new Processor(false);
var comp = proc.newXsltCompiler();
string code = args[0], sch = args[1], xml = args[2], outDir = args[3];
var sw = System.Diagnostics.Stopwatch.StartNew();
XdmNode Run(string xsl, XdmNode input) {
  var t = comp.compile(new StreamSource(new JFile(xsl))).load();
  t.setSource(input.asSource());
  var dest = new XdmDestination(); t.setDestination(dest); t.transform(); return dest.getXdmNode();
}
var db = proc.newDocumentBuilder();
var schDoc = db.build(new JFile(sch));
var s1 = Run(Path.Combine(code,"iso_dsdl_include.xsl"), schDoc);
var s2 = Run(Path.Combine(code,"iso_abstract_expand.xsl"), s1);
var s3 = Run(Path.Combine(code,"iso_svrl_for_xslt2.xsl"), s2);
var xslPath = Path.Combine(outDir, Path.GetFileNameWithoutExtension(sch) + ".xslt");
System.IO.File.WriteAllText(xslPath, s3.toString());
System.Console.WriteLine($"compiled {sch} in {sw.ElapsedMilliseconds} ms");
sw.Restart();
var validator = comp.compile(new StreamSource(new JFile(xslPath))).load();
validator.setSource(new StreamSource(new JFile(xml)));
var d = new XdmDestination(); validator.setDestination(d); validator.transform();
var svrl = d.getXdmNode().toString();
System.IO.File.WriteAllText(Path.Combine(outDir, Path.GetFileNameWithoutExtension(sch) + ".svrl.xml"), svrl);
var x = System.Xml.Linq.XDocument.Parse(svrl);
System.Xml.Linq.XNamespace sv = "http://purl.oclc.org/dsdl/svrl";
var fails = x.Descendants(sv+"failed-assert").ToList();
System.Console.WriteLine($"validated in {sw.ElapsedMilliseconds} ms; fired-rules={x.Descendants(sv+"fired-rule").Count()} failed-asserts={fails.Count}");
foreach (var f in fails) System.Console.WriteLine($"  [{f.Attribute("flag")?.Value}] {f.Attribute("id")?.Value}: {f.Value.Trim().Split('\n')[0]}");
