using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Mistruna.Core.HealthChecks;

/// <summary>
/// Generic health check to verify EF Core database connectivity.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DatabaseHealthCheck{TDbContext}"/> class.
/// </remarks>
/// <param name="dbContext">The database context.</param>
/// <param name="logger">The logger instance.</param>
public sealed class DatabaseHealthCheck<TDbContext>(
    TDbContext dbContext,
    ILogger<DatabaseHealthCheck<TDbContext>> logger) : IHealthCheck
    where TDbContext : DbContext
{
    /// <summary>
    /// Performs the health check by attempting to connect to the database.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The health check result.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

            if (canConnect)
            {
                logger.LogDebug("Database health check succeeded");
                return HealthCheckResult.Healthy("Database is accessible");
            }

            logger.LogWarning("Database health check failed: Cannot connect");
            return HealthCheckResult.Unhealthy("Cannot connect to database");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database health check failed with exception");
            return HealthCheckResult.Unhealthy("Database health check failed", ex);
        }
    }
}
