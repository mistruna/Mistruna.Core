namespace Mistruna.Core.Metering;

/// <summary>
/// Records successful API-key usage events. Called fire-and-forget from the
/// <see cref="UsageMeteringMiddleware"/>. Implementations MUST NOT throw for normal
/// infrastructure outages — the middleware wraps calls in a try/catch but returning gracefully
/// keeps warning noise low.
/// </summary>
public interface IUsageMeter
{
    Task IncrementAsync(Guid apiKeyId, DateOnly date, CancellationToken cancellationToken);
}
