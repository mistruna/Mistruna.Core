namespace Mistruna.Core.Metering;

/// <summary>
/// Configuration options for usage metering middleware.
/// </summary>
public sealed class UsageMeteringOptions
{
    /// <summary>
    /// Predicate that determines which HTTP status codes count as billable usage.
    /// Defaults to 2xx responses.
    /// </summary>
    public Func<int, bool> IsSuccessful { get; set; } = status => status is >= 200 and < 300;
}
