namespace Mistruna.Core.Authentication.ApiKey;

/// <summary>
/// Claim type names set by <see cref="ApiKeyAuthenticationHandler"/> on the authenticated principal.
/// </summary>
public static class ApiKeyClaimTypes
{
    /// <summary>
    /// Subject claim — the API key's owning user ID (Guid string).
    /// </summary>
    public const string Subject = "sub";

    /// <summary>
    /// The unique identifier of the API key itself.
    /// </summary>
    public const string ApiKeyId = "apikey_id";
}
