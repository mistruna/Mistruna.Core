namespace Mistruna.Core.Monetization.Authentication.ApiKey;

/// <summary>
/// Resolves an inbound API-key string to an <see cref="ApiKeyValidationResult"/>.
/// Implementations are expected to cache via Redis (keyed by SHA-256 of the key) and
/// fall back to an RPC against user-service on cache miss (see spec § 3 AD-3).
/// </summary>
public interface IApiKeyValidator
{
    Task<ApiKeyValidationResult> ValidateAsync(string apiKey, CancellationToken cancellationToken);
}
