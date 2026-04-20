namespace Mistruna.Core.Authorization.Plans;

/// <summary>
/// Subscription tiers in ascending order of capability.
/// Ordering is load-bearing: <see cref="RequiresPlanAuthorizationHandler"/> compares
/// the current user's tier against the required tier using the underlying integer value.
/// </summary>
public enum Plan
{
    Free = 0,
    Developer = 1,
    Pro = 2,
    Enterprise = 3
}
