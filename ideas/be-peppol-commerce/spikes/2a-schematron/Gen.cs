public static class Gen { public static void Emit(string json, string outp) {
  var o = BePeppolCommerce.Core.Model.OrderJson.Parse(System.IO.File.ReadAllText(json));
  BePeppolCommerce.Core.Ubl.PeppolInvoiceBuilder.Build(o).Save(outp); } }
