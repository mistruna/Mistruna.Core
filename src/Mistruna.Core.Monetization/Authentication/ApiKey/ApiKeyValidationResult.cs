using Mistruna.Core.Monetization.Authorization.Plans;

namespace Mistruna.Core.Monetization.Authentication.ApiKey;

/// <summary>
/// Outcome of an <see cref="IApiKeyValidator"/> check. Use the static factories rather than
/// constructing directly.
/// </summary>
public sealed class ApiKeyValidationResult
{
    private ApiKeyValidationResult() { }

    public bool IsValid { get; private init; }
    public Guid? UserId { get; private init; }
    public Guid? ApiKeyId { get; private init; }
    public Plan? Tier { get; private init; }
    public long? Quota { get; private init; }
    public string? FailureReason { get; private init; }

    public static ApiKeyValidationResult Success(Guid userId, Guid apiKeyId, Plan tier, long quota) =>
        new() { IsValid = true, UserId = userId, ApiKeyId = apiKeyId, Tier = tier, Quota = quota };

    public static ApiKeyValidationResult Failure(string reason) =>
        new() { IsValid = false, FailureReason = reason };
}
