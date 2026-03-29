namespace Mistruna.Core.Contracts.Models;

public record struct CurrencyInfo(
    string Code,
    short Numeric,
    string FullName,
    string Symbol);
