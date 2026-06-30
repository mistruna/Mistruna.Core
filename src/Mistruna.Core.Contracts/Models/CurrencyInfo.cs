namespace Mistruna.Core.Contracts.Models;

/// <summary>
/// Structured metadata for a currency: code, numeric ISO code, full name, and display symbol.
/// </summary>
/// <param name="Code">ISO 4217 alphabetic currency code (e.g., "USD").</param>
/// <param name="Numeric">ISO 4217 numeric currency code.</param>
/// <param name="FullName">Human-readable full currency name (e.g., "US Dollar").</param>
/// <param name="Symbol">Display symbol (e.g., "$", "€").</param>
public record struct CurrencyInfo(
    string Code,
    short Numeric,
    string FullName,
    string Symbol);
