namespace BePeppolCommerce.Core.Model;

/// <summary>
/// Minimal, platform-neutral order/invoice record. In v1 this comes from a JSON fixture; in a real
/// Optimizely Configured Commerce install it would be mapped from the platform's own order object
/// (see PLAN.md, Slice 6).
/// </summary>
public sealed record Order
{
    public required string InvoiceNumber { get; init; }
    public required DateOnly IssueDate { get; init; }
    public DateOnly? DueDate { get; init; }
    public required string CurrencyCode { get; init; }
    public string? BuyerReference { get; init; }
    public required Party Seller { get; init; }
    public required Party Buyer { get; init; }
    public required IReadOnlyList<OrderLine> Lines { get; init; }
}

public sealed record Party
{
    public required string Name { get; init; }
    /// <summary>Peppol participant identifier value, e.g. a Belgian enterprise number.</summary>
    public required string EndpointId { get; init; }
    /// <summary>Peppol EAS code for <see cref="EndpointId"/>; 0208 = Belgian enterprise number.</summary>
    public required string EndpointSchemeId { get; init; }
    public string? VatNumber { get; init; }
    public required Address Address { get; init; }
}

public sealed record Address
{
    public string? Street { get; init; }
    public string? City { get; init; }
    public string? PostalCode { get; init; }
    /// <summary>ISO 3166-1 alpha-2 country code.</summary>
    public required string CountryCode { get; init; }
}

public sealed record OrderLine
{
    public required string Id { get; init; }
    public required string Description { get; init; }
    public required decimal Quantity { get; init; }
    /// <summary>UN/ECE Rec 20 unit code, e.g. "C62" (one/unit).</summary>
    public string UnitCode { get; init; } = "C62";
    public required decimal UnitPrice { get; init; }
    /// <summary>VAT category code (UNCL5305), e.g. "S" = standard rate.</summary>
    public string VatCategory { get; init; } = "S";
    public required decimal VatPercent { get; init; }

    public decimal LineExtensionAmount => Math.Round(Quantity * UnitPrice, 2, MidpointRounding.AwayFromZero);
}
