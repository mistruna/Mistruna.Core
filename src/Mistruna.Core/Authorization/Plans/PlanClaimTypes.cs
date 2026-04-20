namespace Mistruna.Core.Authorization.Plans;

/// <summary>
/// Claim type constants used to surface subscription tier + quota on the current principal.
/// Populated by <see cref="Authentication.ApiKey.ApiKeyAuthenticationHandler"/> and by
/// the JWT issued by user-service on the `/api/internal/tokens/exchange` endpoint.
/// </summary>
public static class PlanClaimTypes
{
    public const string Tier = "tier";
    public const string Quota = "quota";
}
