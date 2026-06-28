namespace Mistruna.Core.Metering;

public sealed class UsageMeteringOptions
{
    /// <summary>
    /// HTTP status codes counted as "successful" usage.
    /// Defaults to [200, 300) — suitable for tiered API billing policies.
    /// </summary>
    public Func<int, bool> IsSuccessful { get; set; } = status => status is >= 200 and < 300;
}
