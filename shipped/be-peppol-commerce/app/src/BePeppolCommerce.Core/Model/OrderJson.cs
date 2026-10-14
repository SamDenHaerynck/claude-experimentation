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
            || order.Lines.Any(l => l is null) || (order.AllowancesCharges?.Any(a => a is null) ?? false))
            throw new JsonException("Order JSON is missing seller, buyer, an address, or lines.");
        order = order with { Seller = Normalise(order.Seller), Buyer = Normalise(order.Buyer), Payment = NormalisePayment(order.Payment) };
        var missing = MissingRequiredStrings(order).FirstOrDefault();
        if (missing is not null)
            throw new JsonException($"Order JSON has a null or empty required value: {missing}.");
        return order;
    }

    private static PaymentInstruction? NormalisePayment(PaymentInstruction? p)
    {
        if (p is null) return null;
        var iban = PaymentReferences.NormaliseIban(p.Iban)
            ?? throw new JsonException("Order JSON has an invalid payment.iban (failed the IBAN checksum).");
        var ogm = p.StructuredCommunication is null ? null
            : PaymentReferences.NormaliseStructuredCommunication(p.StructuredCommunication)
                ?? throw new JsonException("Order JSON has an invalid payment.structuredCommunication (needs 12 digits with a valid modulo-97 check).");
        return p with { Iban = iban, StructuredCommunication = ogm };
    }

    // A Belgian enterprise number is often written 0403.170.701 or "0403 170 701"; Peppol wants digits only
    // (PEPPOL-COMMON-R043). Only the 0208 endpoint id and the legal registration id are touched.
    private static Party Normalise(Party p)
    {
        static string Digits(string v) => new(v.Where(char.IsAsciiDigit).ToArray());
        static bool Formatted(string v) => v.Length > 0 && v.All(c => char.IsAsciiDigit(c) || c is '.' or ' ');
        var endpoint = p.EndpointSchemeId == "0208" && p.EndpointId is { } e && Formatted(e) ? Digits(e) : p.EndpointId;
        var reg = p.LegalRegistrationId is { } r && p.Address?.CountryCode == "BE" && Formatted(r) ? Digits(r) : p.LegalRegistrationId;
        return p with { EndpointId = endpoint, LegalRegistrationId = reg };
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
        for (var i = 0; i < (o.AllowancesCharges?.Count ?? 0); i++)
        {
            var a = o.AllowancesCharges![i];
            if (Blank(a.VatCategory)) yield return $"allowancesCharges[{i}].vatCategory";
            if (a.Amount < 0) yield return $"allowancesCharges[{i}].amount (must not be negative)";
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
