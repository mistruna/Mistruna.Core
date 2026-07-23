namespace Mistruna.Core.Monetization.Authorization.Plans;

/// <summary>
/// Claim type names used to surface subscription tier and quota on the current principal.
/// Populated by <see cref="Authentication.ApiKey.ApiKeyAuthenticationHandler"/> and JWT tokens.
/// </summary>
public static class PlanClaimTypes
{
    /// <summary>
    /// The subscription tier claim (e.g., "Free", "Developer", "Pro", "Enterprise").
    /// </summary>
    public const string Tier = "tier";

    /// <summary>
    /// The daily request quota for the current plan.
    /// </summary>
    public const string Quota = "quota";
}
