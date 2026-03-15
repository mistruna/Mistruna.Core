using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Mistruna.Core.Caching;
using Mistruna.Core.Contracts.Base.Infrastructure;
using Mistruna.Core.HealthChecks;
using StackExchange.Redis;

namespace Mistruna.Core.Extensions;

/// <summary>
/// Extension methods for registering Redis caching and health check services.
/// </summary>
public static class RedisServiceCollectionExtensions
{
    /// <summary>
    /// Registers a Redis <see cref="IConnectionMultiplexer"/> singleton and the
    /// <see cref="RedisCacheService"/> as the <see cref="ICacheService"/> implementation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The Redis connection string (e.g., "localhost:6379").</param>
    /// <param name="configureCacheOptions">Optional delegate to configure <see cref="CacheOptions"/>.</param>
    /// <returns>The service collection.</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddRedisCaching("localhost:6379", opts =>
    /// {
    ///     opts.KeyPrefix = "myapp";
    ///     opts.DefaultExpiration = TimeSpan.FromMinutes(10);
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        string connectionString,
        Action<CacheOptions>? configureCacheOptions = null)
    {
        var cacheOptions = new CacheOptions();
        configureCacheOptions?.Invoke(cacheOptions);

        services.AddSingleton(cacheOptions);

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(connectionString));

        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }

    /// <summary>
    /// Adds a Redis health check to the health checks builder.
    /// </summary>
    /// <param name="builder">The health checks builder.</param>
    /// <param name="name">The health check name. Defaults to "redis".</param>
    /// <param name="failureStatus">The failure status. Defaults to <see cref="HealthStatus.Unhealthy"/>.</param>
    /// <param name="tags">Optional tags for the health check.</param>
    /// <returns>The health checks builder.</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddHealthChecks()
    ///     .AddRedisHealthCheck();
    /// </code>
    /// </example>
    public static IHealthChecksBuilder AddRedisHealthCheck(
        this IHealthChecksBuilder builder,
        string name = "redis",
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null)
    {
        return builder.AddCheck<RedisHealthCheck>(
            name,
            failureStatus ?? HealthStatus.Unhealthy,
            tags ?? ["redis", "cache", "infrastructure"]);
    }
}
