using Mistruna.Core.Enums;

namespace Mistruna.Core.Extensions;

/// <summary>
/// Provides extension methods for working with enum types.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the currency symbol for a given currency code.
    /// </summary>
    /// <param name="currencyCode">The currency code to get the symbol for.</param>
    /// <returns>The currency symbol (e.g., "$" for USD, "€" for EUR).</returns>
    /// <example>
    /// <code>
    /// var symbol = Currency.Usd.GetCurrencySymbol(); // Returns "$"
    /// </code>
    /// </example>
    public static string GetCurrencySymbol(this Currency currencyCode)
        => currencyCode switch
        {
            Currency.Aed => "د.إ",
            Currency.Aud => "A$",
            Currency.Bdt => "৳",
            Currency.Bgn => "лв",
            Currency.Bnd => "B$",
            Currency.Brl => "R$",
            Currency.Byn => "Br",
            Currency.Cad => "C$",
            Currency.Chf => "CHF",
            Currency.Cny => "¥",
            Currency.Czk => "Kč",
            Currency.Dkk => "kr",
            Currency.Egp => "£",
            Currency.Eur => "€",
            Currency.Gbp => "£",
            Currency.Hkd => "HK$",
            Currency.Huf => "Ft",
            Currency.Idr => "Rp",
            Currency.Ils => "₪",
            Currency.Inr => "₹",
            Currency.Isk => "kr",
            Currency.Jmd => "J$",
            Currency.Jpy => "¥",
            Currency.Krw => "₩",
            Currency.Kwd => "د.ك",
            Currency.Lkr => "Rs",
            Currency.Mxn => "$",
            Currency.Myr => "RM",
            Currency.Nok => "kr",
            Currency.Nzd => "NZ$",
            Currency.Php => "₱",
            Currency.Pkr => "₨",
            Currency.Pln => "zł",
            Currency.Ron => "lei",
            Currency.Rub => "₽",
            Currency.Sar => "﷼",
            Currency.Sek => "kr",
            Currency.Sgd => "S$",
            Currency.Thb => "฿",
            Currency.Try => "₺",
            Currency.Twd => "NT$",
            Currency.Uah => "₴",
            Currency.Usd => "$",
            Currency.Uzs => "сўм",
            Currency.Vnd => "₫",
            Currency.Zar => "R",
            _ => Enum.GetName(currencyCode) ?? currencyCode.ToString()
        };
}
