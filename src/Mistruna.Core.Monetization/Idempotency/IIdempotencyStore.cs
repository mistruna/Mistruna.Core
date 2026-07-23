namespace Mistruna.Core.Monetization.Idempotency;

/// <summary>
/// Persists and retrieves <see cref="IdempotentResponse"/> snapshots keyed by the combination
/// of user identity and client-supplied Idempotency-Key (formed by the middleware).
/// </summary>
public interface IIdempotencyStore
{
    Task<IdempotentResponse?> GetAsync(string key, CancellationToken cancellationToken);
    Task SetAsync(string key, IdempotentResponse response, TimeSpan ttl, CancellationToken cancellationToken);
}
