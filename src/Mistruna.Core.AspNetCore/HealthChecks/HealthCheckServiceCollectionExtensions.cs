using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mistruna.Core.AspNetCore.HealthChecks;

public static class HealthCheckServiceCollectionExtensions
{
    public static IServiceCollection AddMistrunaHealthChecks(
        this IServiceCollection services,
        Action<IHealthChecksBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live", "ready"]);

        configure?.Invoke(builder);
        return services;
    }

    public static IHealthChecksBuilder AddMistrunaDatabaseHealthCheck<TDbContext>(
        this IHealthChecksBuilder builder,
        string name = "database",
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null)
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddCheck<DatabaseHealthCheck<TDbContext>>(
            name,
            failureStatus ?? HealthStatus.Unhealthy,
            tags ?? ["ready", "db", "infrastructure"]);
    }
}
