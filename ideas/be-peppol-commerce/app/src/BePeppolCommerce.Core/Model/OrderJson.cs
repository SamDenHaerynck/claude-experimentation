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
        return order;
    }
}
