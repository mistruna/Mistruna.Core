using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mistruna.Core.Idempotency;
using Mistruna.Core.Metering;
using Mistruna.Core.RateLimiting.Tiered;

namespace Mistruna.Core.Extensions;

public static class MonetizationApplicationBuilderExtensions
{
    /// <summary>
    /// Adds <see cref="TieredRateLimitMiddleware"/> to the pipeline. Must run AFTER
    /// <c>UseAuthentication()</c> so the principal's tier/quota claims are available.
    /// </summary>
    public static IApplicationBuilder UseTieredRateLimiting(this IApplicationBuilder app) =>
        app.Use((context, next) =>
        {
            var options = context.RequestServices.GetRequiredService<TieredRateLimitOptions>();
            var store = context.RequestServices.GetRequiredService<IQuotaStore>();
            var logger = context.RequestServices.GetRequiredService<ILogger<TieredRateLimitMiddleware>>();
            return new TieredRateLimitMiddleware(_ => next(context), options, store, logger).InvokeAsync(context);
        });

    public static IApplicationBuilder UseIdempotency(this IApplicationBuilder app) =>
        app.Use((context, next) =>
        {
            var options = context.RequestServices.GetRequiredService<IdempotencyOptions>();
            var store = context.RequestServices.GetRequiredService<IIdempotencyStore>();
            var logger = context.RequestServices.GetRequiredService<ILogger<IdempotencyMiddleware>>();
            return new IdempotencyMiddleware(_ => next(context), options, store, logger).InvokeAsync(context);
        });

    public static IApplicationBuilder UseUsageMetering(this IApplicationBuilder app) =>
        app.Use((context, next) =>
        {
            var options = context.RequestServices.GetRequiredService<UsageMeteringOptions>();
            var meter = context.RequestServices.GetRequiredService<IUsageMeter>();
            var logger = context.RequestServices.GetRequiredService<ILogger<UsageMeteringMiddleware>>();
            return new UsageMeteringMiddleware(_ => next(context), options, meter, logger).InvokeAsync(context);
        });
}
