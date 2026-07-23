using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mistruna.Core.Monetization.Authentication.ApiKey;
using Mistruna.Core.Monetization.Authorization.Plans;
using Mistruna.Core.Monetization.Idempotency;
using Mistruna.Core.Monetization.Metering;
using Mistruna.Core.Monetization.RateLimiting.Tiered;

namespace Mistruna.Core.Monetization.DependencyInjection;

/// <summary>
/// Registration helpers for the Mistruna.Core monetization primitives.
/// Each call is independent — register only what you need.
/// </summary>
public static class MonetizationServiceCollectionExtensions
{
    /// <summary>Registers the complete Mistruna monetization stack.</summary>
    public static IServiceCollection AddMistrunaMonetization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMistrunaApiKeyAuthentication();
        services.AddMistrunaPlanAuthorization();
        services.AddMistrunaTieredRateLimiting();
        services.AddMistrunaIdempotency();
        services.AddMistrunaUsageMetering();

        return services;
    }

    /// <summary>
    /// Registers the <c>ApiKey</c> authentication scheme. The consumer must also register an
    /// <see cref="IApiKeyValidator"/> implementation (typically a Redis-cached delegate to the
    /// user-service token-exchange endpoint).
    /// </summary>
    public static IServiceCollection AddMistrunaApiKeyAuthentication(
        this IServiceCollection services,
        Action<ApiKeyAuthenticationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddAuthentication().AddMistrunaApiKeyAuthentication(configure);
        return services;
    }

    /// <summary>
    /// Adds the Mistruna API-key scheme to an existing authentication builder.
    /// </summary>
    public static AuthenticationBuilder AddMistrunaApiKeyAuthentication(
        this AuthenticationBuilder builder,
        Action<ApiKeyAuthenticationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            ApiKeyDefaults.AuthenticationScheme,
            configure ?? (_ => { }));
    }

    /// <summary>
    /// Registers <see cref="RequiresPlanAuthorizationHandler"/>, the dynamic
    /// <see cref="RequiresPlanPolicyProvider"/>, and the
    /// <see cref="PlanAuthorizationMiddlewareResultHandler"/> that translates a failed
    /// <see cref="RequiresPlanRequirement"/> into HTTP 402 Payment Required.
    /// Call after <c>AddAuthorization()</c>.
    /// </summary>
    public static IServiceCollection AddMistrunaPlanAuthorization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddAuthorization();
        services.AddSingleton<IAuthorizationHandler, RequiresPlanAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, RequiresPlanPolicyProvider>();
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, PlanAuthorizationMiddlewareResultHandler>();
        return services;
    }

    /// <summary>
    /// Registers <see cref="TieredRateLimitOptions"/> and uses <c>TryAdd</c> to register
    /// <see cref="RedisQuotaStore"/> as <see cref="IQuotaStore"/>. The Redis implementation
    /// requires an <c>IConnectionMultiplexer</c>; register a custom <see cref="IQuotaStore"/>
    /// before calling this method to override it.
    /// </summary>
    public static IServiceCollection AddMistrunaTieredRateLimiting(
        this IServiceCollection services,
        Action<TieredRateLimitOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

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
        ArgumentNullException.ThrowIfNull(services);

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
        ArgumentNullException.ThrowIfNull(services);

        var options = new UsageMeteringOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);
        services.TryAddSingleton<IUsageMeter, RedisUsageMeter>();
        return services;
    }
}
