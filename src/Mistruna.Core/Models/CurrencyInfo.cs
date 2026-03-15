namespace Mistruna.Core.Models;

public readonly record struct CurrencyInfo(
    string Code,
    short Numeric,
    string FullName,
    string Symbol);
