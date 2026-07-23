using Microsoft.AspNetCore.Authentication;

namespace Mistruna.Core.Monetization.Authentication.ApiKey;

public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>Header name containing the key. Defaults to <c>X-Api-Key</c>.</summary>
    public string HeaderName { get; set; } = ApiKeyDefaults.HeaderName;

    /// <summary>
    /// If true, the handler will also accept the key via the <c>?access_token=</c> query
    /// parameter. Required for WebSocket/SignalR negotiation where custom headers cannot
    /// be set from the browser. Defaults to <c>false</c>.
    /// </summary>
    public bool AllowQueryParameter { get; set; }

    /// <summary>Query parameter name when <see cref="AllowQueryParameter"/> is true.</summary>
    public string QueryParameterName { get; set; } = "access_token";
}
