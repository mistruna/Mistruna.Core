using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Mistruna.Core.AspNetCore.HealthChecks;

internal sealed class DatabaseHealthCheck<TDbContext>(
    TDbContext dbContext,
    ILogger<DatabaseHealthCheck<TDbContext>> logger) : IHealthCheck
    where TDbContext : DbContext
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await dbContext.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy("Database is accessible")
                : HealthCheckResult.Unhealthy("Cannot connect to database");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Database health check failed");
            return HealthCheckResult.Unhealthy("Database health check failed", exception);
        }
    }
}
