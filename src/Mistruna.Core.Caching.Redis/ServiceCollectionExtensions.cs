using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Mistruna.Core.Abstractions.Persistence;
using StackExchange.Redis;

namespace Mistruna.Core.Caching.Redis;

/// <summary>Extension methods for registering Redis caching and health check services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Redis caching services using configuration from
    /// <c>Mistruna:Redis</c> or <c>ConnectionStrings:Redis</c>.
    /// </summary>
    public static IServiceCollection AddMistrunaRedis(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<CacheOptions>? configureCacheOptions = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = ResolveConnectionString(configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Redis connection string is not configured. Set Mistruna:Redis:ConnectionString or ConnectionStrings:Redis.");
        }

        services.Configure<CacheOptions>(configuration.GetSection("Mistruna:Redis"));
        if (configureCacheOptions is not null)
        {
            services.PostConfigure(configureCacheOptions);
        }

        services.TryAddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(connectionString));

        services.TryAddSingleton<ICacheService, RedisCacheService>();

        return services;
    }

    /// <summary>Adds a Redis health check to the health checks builder.</summary>
    public static IHealthChecksBuilder AddRedisHealthCheck(
        this IHealthChecksBuilder builder,
        string name = "redis",
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddCheck<RedisHealthCheck>(
            name,
            failureStatus ?? HealthStatus.Unhealthy,
            tags ?? ["redis", "cache", "infrastructure"]);
    }

    private static string? ResolveConnectionString(IConfiguration configuration)
    {
        var mistrunaConnectionString = configuration["Mistruna:Redis:ConnectionString"];
        if (!string.IsNullOrWhiteSpace(mistrunaConnectionString))
        {
            return mistrunaConnectionString;
        }

        return configuration.GetConnectionString("Redis");
    }
}
