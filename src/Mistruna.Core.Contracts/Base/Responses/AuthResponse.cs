namespace Mistruna.Core.Contracts.Base.Responses;

/// <summary>
/// Standard authentication response of Create, Update, Get single operation
/// </summary>
public class AuthResponse
{
    /// <summary>Username</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Email</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Token</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Refresh token</summary>
    public string RefreshToken { get; set; } = string.Empty;
}
