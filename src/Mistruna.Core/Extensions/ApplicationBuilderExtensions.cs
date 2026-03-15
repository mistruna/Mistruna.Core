using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mistruna.Core.Middlewares;
using Mistruna.Core.RateLimiting;
using StackExchange.Redis;

namespace Mistruna.Core.Extensions;

/// <summary>
/// Extension methods for configuring the application builder.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the core exception handling middleware to the pipeline.
    /// This middleware catches exceptions and returns appropriate HTTP responses.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    /// <example>
    /// <code>
    /// app.UseCoreMiddlewares();
    /// </code>
    /// </example>
    public static IApplicationBuilder UseCoreMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        return app;
    }

    /// <summary>
    /// Adds the exception handling middleware to the pipeline.
    /// This middleware catches exceptions and returns appropriate HTTP responses.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();

    /// <summary>
    /// Adds the rate limiting middleware to the pipeline.
    /// Limits requests per IP address within a configurable time window.
    /// Returns HTTP 429 when the limit is exceeded.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="configure">Optional delegate to configure <see cref="RateLimitOptions"/>.</param>
    /// <returns>The application builder.</returns>
    /// <example>
    /// <code>
    /// app.UseRateLimiting(opts =>
    /// {
    ///     opts.RequestsPerWindow = 60;
    ///     opts.WindowSeconds = 30;
    /// });
    /// </code>
    /// </example>
    public static IApplicationBuilder UseRateLimiting(
        this IApplicationBuilder app,
        Action<RateLimitOptions>? configure = null)
    {
        var options = new RateLimitOptions();
        configure?.Invoke(options);

        app.Use((context, next) =>
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<RateLimitingMiddleware>>();
            var redis = context.RequestServices.GetService<IConnectionMultiplexer>();
            var middleware = new RateLimitingMiddleware(next, options, logger, redis);
            return middleware.InvokeAsync(context);
        });

        return app;
    }
}
