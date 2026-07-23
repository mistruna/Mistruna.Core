using Microsoft.AspNetCore.Builder;

namespace Mistruna.Core.AspNetCore.RateLimiting;

public static class IpRateLimitingApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMistrunaIpRateLimiting(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<MistrunaIpRateLimitingMiddleware>();
    }
}
