using System.Numerics;

namespace BePeppolCommerce.Core.Model;

/// <summary>Normalisation and checksum checks for IBANs and Belgian structured communications (OGM/VCS).</summary>
public static class PaymentReferences
{
    /// <summary>Removes spaces and upper-cases. Returns null if the result is not a valid IBAN (ISO 13616 mod-97).</summary>
    public static string? NormaliseIban(string? value)
    {
        if (value is null) return null;
        var iban = new string(value.Where(c => c != ' ').ToArray()).ToUpperInvariant();
        if (iban.Length is < 15 or > 34 || !iban.All(char.IsAsciiLetterOrDigit)
            || !char.IsAsciiLetterUpper(iban[0]) || !char.IsAsciiLetterUpper(iban[1])
            || !char.IsAsciiDigit(iban[2]) || !char.IsAsciiDigit(iban[3]))
            return null;
        var rearranged = iban[4..] + iban[..4];
        var digits = string.Concat(rearranged.Select(c => char.IsAsciiDigit(c) ? c.ToString() : (c - 'A' + 10).ToString()));
        return BigInteger.Remainder(BigInteger.Parse(digits), 97) == 1 ? iban : null;
    }

    /// <summary>
    /// Accepts "+++123/4567/89002+++", "***...***" or bare digits (12 digits; separators and spaces ignored) and returns
    /// the canonical "+++xxx/xxxx/xxxxx+++" form, or null if the length or check digits are wrong.
    /// </summary>
    public static string? NormaliseStructuredCommunication(string? value)
    {
        if (value is null) return null;
        var stripped = value.Trim().Trim('+', '*').Replace("/", "").Replace(" ", "");
        if (stripped.Length != 12 || !stripped.All(char.IsAsciiDigit)) return null;
        var number = long.Parse(stripped[..10]);
        var check = int.Parse(stripped[10..]);
        var expected = (int)(number % 97);
        if (expected == 0) expected = 97;
        return check == expected ? $"+++{stripped[..3]}/{stripped[3..7]}/{stripped[7..]}+++" : null;
    }
}
