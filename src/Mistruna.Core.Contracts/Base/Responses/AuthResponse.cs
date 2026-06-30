namespace Mistruna.Core.Contracts.Base.Responses;

/// <summary>
/// Standard authentication response returned after login, token refresh, or registration.
/// </summary>
public class AuthResponse
{
    /// <summary>The authenticated user's username.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>The authenticated user's email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>JWT access token for subsequent API calls.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Refresh token used to obtain a new access token without re-authenticating.</summary>
    public string RefreshToken { get; set; } = string.Empty;
}
