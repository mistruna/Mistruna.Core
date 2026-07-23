using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace Mistruna.Core.Resilience.Internal;

internal static class MistrunaHttpResiliencePipelineConfigurator
{
    public static void ConfigureStandard(ResiliencePipelineBuilder<HttpResponseMessage> builder)
        => Configure(builder, new HttpStandardResilienceOptions());

    public static void ConfigureAggressive(ResiliencePipelineBuilder<HttpResponseMessage> builder)
    {
        var options = new HttpStandardResilienceOptions();
        options.Retry.MaxRetryAttempts = 5;
        options.Retry.Delay = TimeSpan.FromMilliseconds(200);
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
        Configure(builder, options);
    }

    private static void Configure(
        ResiliencePipelineBuilder<HttpResponseMessage> builder,
        HttpStandardResilienceOptions options)
    {
        builder
            .AddRateLimiter(options.RateLimiter)
            .AddTimeout(options.TotalRequestTimeout)
            .AddRetry(options.Retry)
            .AddCircuitBreaker(options.CircuitBreaker)
            .AddTimeout(options.AttemptTimeout);
    }
}
