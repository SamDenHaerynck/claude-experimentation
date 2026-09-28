using System.Text.Json;

namespace BePeppolCommerce.Core.Model;

public static class OrderJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>Parses an order from JSON. Throws <see cref="JsonException"/> on malformed input.</summary>
    public static Order Parse(string json) =>
        JsonSerializer.Deserialize<Order>(json, Options)
        ?? throw new JsonException("Order JSON was null.");
}
