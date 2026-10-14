using System.Xml; using System.Xml.Schema;
public static class Xsd { public static void Check(string xsd, string xml) {
  var set = new XmlSchemaSet(); set.XmlResolver = new XmlUrlResolver(); using (var xr = XmlReader.Create(xsd, new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse, XmlResolver = new XmlUrlResolver() })) set.Add(null, xr); set.Compile();
  var st = new XmlReaderSettings { ValidationType = ValidationType.Schema, Schemas = set };
  int n = 0; st.ValidationEventHandler += (s, e) => { n++; System.Console.WriteLine($"  [{e.Severity}] {e.Message}"); };
  using (var r = XmlReader.Create(xml, st)) while (r.Read()) { }
  System.Console.WriteLine($"XSD {System.IO.Path.GetFileName(xml)}: {n} issues"); } }
