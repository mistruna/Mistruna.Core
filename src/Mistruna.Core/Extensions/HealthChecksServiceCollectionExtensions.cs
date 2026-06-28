using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Mistruna.Core.HealthChecks;

namespace Mistruna.Core.Extensions;

/// <summary>
/// Opinionated health check registration helpers for ASP.NET Core applications.
/// </summary>
public static class HealthChecksServiceCollectionExtensions
{
    /// <summary>
    /// Registers the default Mistruna.Core health checks.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional callback to add more checks to the builder.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// Always adds a lightweight self check named <c>self</c>.
    /// Use <see cref="AddDatabaseHealthCheck{TDbContext}"/> and
    /// <see cref="RedisServiceCollectionExtensions.AddRedisHealthCheck"/> for infrastructure checks.
    /// </remarks>
    public static IServiceCollection AddCoreHealthChecks(
        this IServiceCollection services,
        Action<IHealthChecksBuilder>? configure = null)
    {
        var builder = services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live", "ready"]);

        configure?.Invoke(builder);
        return services;
    }

    /// <summary>
    /// Adds an EF Core database connectivity check for the given <typeparamref name="TDbContext"/>.
    /// </summary>
    public static IHealthChecksBuilder AddDatabaseHealthCheck<TDbContext>(
        this IHealthChecksBuilder builder,
        string name = "database",
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null)
        where TDbContext : DbContext
    {
        return builder.AddCheck<DatabaseHealthCheck<TDbContext>>(
            name,
            failureStatus ?? HealthStatus.Unhealthy,
            tags ?? ["ready", "db", "infrastructure"]);
    }
}
