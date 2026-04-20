using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Mistruna.Core.Authorization.Plans;

/// <summary>
/// Dynamic policy provider that synthesizes an <see cref="AuthorizationPolicy"/> from a
/// "RequiresPlan:{PlanName}" policy name on demand. This lets consumers apply
/// <see cref="RequiresPlanAttribute"/> without pre-registering every policy variant
/// in <c>Program.cs</c>.
/// </summary>
/// <remarks>
/// Delegates non-matching policy names to <see cref="DefaultAuthorizationPolicyProvider"/>
/// so other attributes (e.g., existing <c>[Authorize]</c>) continue to work unchanged.
/// </remarks>
public sealed class RequiresPlanPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    private const string PolicyPrefix = "RequiresPlan:";

    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PolicyPrefix, StringComparison.Ordinal)
            && Enum.TryParse<Plan>(policyName[PolicyPrefix.Length..], ignoreCase: true, out var plan))
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new RequiresPlanRequirement(plan))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }
}
