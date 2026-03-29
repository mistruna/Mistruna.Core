namespace Mistruna.Core.Microservices.Core.JwtAuth;

/// <summary>
/// Role name constants used in JWT claims and authorization policies.
/// </summary>
public static class RoleType
{
    public const string Member = nameof(Member);
    public const string Moderator = nameof(Moderator);
    public const string Administrator = nameof(Administrator);
}
