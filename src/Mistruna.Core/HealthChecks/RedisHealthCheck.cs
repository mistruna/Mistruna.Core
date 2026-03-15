using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Mistruna.Core.HealthChecks;

/// <summary>
/// Health check that verifies Redis connectivity by issuing a PING command.
/// Reports <see cref="HealthStatus.Healthy"/> when Redis responds, otherwise <see cref="HealthStatus.Unhealthy"/>.
/// </summary>
/// <param name="connection">The Redis connection multiplexer.</param>
/// <param name="logger">The logger instance.</param>
public sealed class RedisHealthCheck(
    IConnectionMultiplexer connection,
    ILogger<RedisHealthCheck> logger) : IHealthCheck
{
    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var database = connection.GetDatabase();
            var latency = await database.PingAsync();

            logger.LogDebug("Redis health check succeeded. Latency: {Latency}ms", latency.TotalMilliseconds);

            return HealthCheckResult.Healthy(
                $"Redis is accessible. Latency: {latency.TotalMilliseconds:F1}ms",
                new Dictionary<string, object>
                {
                    ["latency_ms"] = latency.TotalMilliseconds
                });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Redis health check failed");
            return HealthCheckResult.Unhealthy("Redis health check failed", ex);
        }
    }
}
