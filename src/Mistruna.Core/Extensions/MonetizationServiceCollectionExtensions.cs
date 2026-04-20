using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mistruna.Core.Authentication.ApiKey;
using Mistruna.Core.Authorization.Plans;
using Mistruna.Core.Idempotency;
using Mistruna.Core.Metering;
using Mistruna.Core.RateLimiting.Tiered;

namespace Mistruna.Core.Extensions;

/// <summary>
/// Registration helpers for the Mistruna.Core monetization primitives.
/// Each call is independent — register only what you need.
/// </summary>
public static class MonetizationServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <c>ApiKey</c> authentication scheme. The consumer must also register an
    /// <see cref="IApiKeyValidator"/> implementation (typically a Redis-cached delegate to the
    /// user-service token-exchange endpoint).
    /// </summary>
    public static AuthenticationBuilder AddMistrunaApiKeyAuthentication(
        this AuthenticationBuilder builder,
        Action<ApiKeyAuthenticationOptions>? configure = null) =>
        builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            ApiKeyDefaults.AuthenticationScheme,
            configure ?? (_ => { }));

    /// <summary>
    /// Registers <see cref="RequiresPlanAuthorizationHandler"/>, the dynamic
    /// <see cref="RequiresPlanPolicyProvider"/>, and the
    /// <see cref="PlanAuthorizationMiddlewareResultHandler"/> that translates a failed
    /// <see cref="RequiresPlanRequirement"/> into HTTP 402 Payment Required.
    /// Call after <c>AddAuthorization()</c>.
    /// </summary>
    public static IServiceCollection AddMistrunaPlanAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, RequiresPlanAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, RequiresPlanPolicyProvider>();
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, PlanAuthorizationMiddlewareResultHandler>();
        return services;
    }

    /// <summary>
    /// Registers <see cref="IQuotaStore"/> (defaults to <see cref="RedisQuotaStore"/> if
    /// <c>IConnectionMultiplexer</c> is available — otherwise <see cref="InMemoryQuotaStore"/>)
    /// and the <see cref="TieredRateLimitOptions"/> singleton.
    /// </summary>
    public static IServiceCollection AddMistrunaTieredRateLimiting(
        this IServiceCollection services,
        Action<TieredRateLimitOptions>? configure = null)
    {
        var options = new TieredRateLimitOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);
        services.TryAddSingleton<IQuotaStore, RedisQuotaStore>();
        return services;
    }

    public static IServiceCollection AddMistrunaIdempotency(
        this IServiceCollection services,
        Action<IdempotencyOptions>? configure = null)
    {
        var options = new IdempotencyOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);
        services.TryAddSingleton<IIdempotencyStore, RedisIdempotencyStore>();
        return services;
    }

    public static IServiceCollection AddMistrunaUsageMetering(
        this IServiceCollection services,
        Action<UsageMeteringOptions>? configure = null)
    {
        var options = new UsageMeteringOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);
        services.TryAddSingleton<IUsageMeter, RedisUsageMeter>();
        return services;
    }
}
