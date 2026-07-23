using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mistruna.Core.Monetization.Authorization.Plans;

namespace Mistruna.Core.Monetization.Authentication.ApiKey;

/// <summary>
/// ASP.NET Core authentication handler for the <see cref="ApiKeyDefaults.AuthenticationScheme"/>
/// scheme. Reads the key from <see cref="ApiKeyAuthenticationOptions.HeaderName"/> (or the
/// configured query parameter when <see cref="ApiKeyAuthenticationOptions.AllowQueryParameter"/>
/// is enabled — typically only for WebSocket/SignalR negotiation), delegates validation to
/// <see cref="IApiKeyValidator"/> resolved from the request services, and projects the result
/// onto a <see cref="ClaimsPrincipal"/> carrying <c>sub</c>, <c>apikey_id</c>, <c>tier</c>, and
/// <c>quota</c> claims used by downstream authorization + rate limiting.
/// </summary>
public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var key = ExtractKey();
        if (key is null)
            return AuthenticateResult.NoResult();

        var validator = Context.RequestServices.GetRequiredService<IApiKeyValidator>();
        var result = await validator.ValidateAsync(key, Context.RequestAborted);

        if (!result.IsValid)
        {
            Logger.LogDebug("ApiKey rejected: {Reason}", result.FailureReason);
            return AuthenticateResult.Fail($"Invalid API key: {result.FailureReason}");
        }

        var identity = new ClaimsIdentity(ApiKeyDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ApiKeyClaimTypes.Subject, result.UserId!.Value.ToString()));
        identity.AddClaim(new Claim(ApiKeyClaimTypes.ApiKeyId, result.ApiKeyId!.Value.ToString()));
        identity.AddClaim(new Claim(PlanClaimTypes.Tier, result.Tier!.Value.ToString()));
        identity.AddClaim(new Claim(PlanClaimTypes.Quota, result.Quota!.Value.ToString()));

        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    private string? ExtractKey()
    {
        if (Context.Request.Headers.TryGetValue(Options.HeaderName, out var headerValue))
        {
            var headerKey = headerValue.ToString();
            if (!string.IsNullOrWhiteSpace(headerKey))
                return headerKey;
        }

        if (Options.AllowQueryParameter
            && Context.Request.Query.TryGetValue(Options.QueryParameterName, out var queryValue))
        {
            var queryKey = queryValue.ToString();
            if (!string.IsNullOrWhiteSpace(queryKey))
                return queryKey;
        }

        return null;
    }
}
