using System.Text.Json;

namespace BePeppolCommerce.Core.Model;

public static class OrderJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>Parses an order from JSON. Throws <see cref="JsonException"/> on malformed input.</summary>
    public static Order Parse(string json)
    {
        var order = JsonSerializer.Deserialize<Order>(json, Options)
            ?? throw new JsonException("Order JSON was null.");
        // `required` does not reject explicit JSON nulls, so check the members Build depends on.
        if (order.Seller?.Address is null || order.Buyer?.Address is null || order.Lines is null
            || order.Lines.Any(l => l is null))
            throw new JsonException("Order JSON is missing seller, buyer, an address, or lines.");
        var missing = MissingRequiredStrings(order).FirstOrDefault();
        if (missing is not null)
            throw new JsonException($"Order JSON has a null or empty required value: {missing}.");
        return order;
    }

    private static IEnumerable<string> MissingRequiredStrings(Order o)
    {
        static bool Blank(string? v) => string.IsNullOrWhiteSpace(v);
        if (Blank(o.InvoiceNumber)) yield return "invoiceNumber";
        if (Blank(o.CurrencyCode)) yield return "currencyCode";
        foreach (var (name, p) in new[] { ("seller", o.Seller), ("buyer", o.Buyer) })
        {
            if (Blank(p.Name)) yield return $"{name}.name";
            if (Blank(p.EndpointId)) yield return $"{name}.endpointId";
            if (Blank(p.EndpointSchemeId)) yield return $"{name}.endpointSchemeId";
            if (Blank(p.Address.CountryCode)) yield return $"{name}.address.countryCode";
        }
        for (var i = 0; i < o.Lines.Count; i++)
        {
            var l = o.Lines[i];
            if (Blank(l.Id)) yield return $"lines[{i}].id";
            if (Blank(l.Description)) yield return $"lines[{i}].description";
            if (Blank(l.UnitCode)) yield return $"lines[{i}].unitCode";
            if (Blank(l.VatCategory)) yield return $"lines[{i}].vatCategory";
        }
    }
}
