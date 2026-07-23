using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mistruna.Core.Behaviors;

/// <summary>Logs request execution and elapsed time.</summary>
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int SlowRequestThresholdMs = 500;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString("N")[..8];

        logger.LogInformation(
            "[{RequestId}] Starting request {RequestName}",
            requestId,
            requestName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next(cancellationToken);
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
            {
                logger.LogWarning(
                    "[{RequestId}] Slow request {RequestName} completed in {ElapsedMs}ms",
                    requestId,
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logger.LogInformation(
                    "[{RequestId}] Completed request {RequestName} in {ElapsedMs}ms",
                    requestId,
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            logger.LogError(
                exception,
                "[{RequestId}] Request {RequestName} failed after {ElapsedMs}ms",
                requestId,
                requestName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
