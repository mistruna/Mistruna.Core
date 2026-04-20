namespace Mistruna.Core.Metering;

public sealed class UsageMeteringOptions
{
    /// <summary>
    /// HTTP status codes counted as "successful" usage.
    /// Defaults to [200, 300) — same policy as Stripe API Gateway billing.
    /// </summary>
    public Func<int, bool> IsSuccessful { get; set; } = status => status is >= 200 and < 300;
}
