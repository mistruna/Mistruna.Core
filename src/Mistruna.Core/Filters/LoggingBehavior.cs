using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mistruna.Core.Filters;

/// <summary>
/// MediatR pipeline behavior that logs request execution time and details.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
/// <remarks>
/// This behavior logs the start and completion of each request, including execution time.
/// Requests that take longer than 500ms are logged as warnings.
///
/// Register this behavior in your DI container:
/// <code>
/// services.AddTransient(typeof(IPipelineBehavior&lt;,&gt;), typeof(LoggingBehavior&lt;,&gt;));
/// </code>
/// </remarks>
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int SlowRequestThresholdMs = 500;

    /// <summary>
    /// Handles the logging of request execution.
    /// </summary>
    /// <param name="request">The request being handled.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the next handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString("N")[..8];

        logger.LogInformation(
            "[{RequestId}] Starting request {RequestName}",
            requestId, requestName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next(cancellationToken);

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (elapsedMs > SlowRequestThresholdMs)
            {
                logger.LogWarning(
                    "[{RequestId}] Slow request {RequestName} completed in {ElapsedMs}ms",
                    requestId, requestName, elapsedMs);
            }
            else
            {
                logger.LogInformation(
                    "[{RequestId}] Completed request {RequestName} in {ElapsedMs}ms",
                    requestId, requestName, elapsedMs);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex,
                "[{RequestId}] Request {RequestName} failed after {ElapsedMs}ms",
                requestId, requestName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
