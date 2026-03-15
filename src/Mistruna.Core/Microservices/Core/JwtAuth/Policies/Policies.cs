namespace Mistruna.Core.Microservices.Core.JwtAuth.Policies;

public static class Policies
{
    public const string ModeratorOnly = nameof(ModeratorOnly);
    public const string MemberOnly = nameof(MemberOnly);
    public const string AdminOnly = nameof(AdminOnly);
}
