using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Mistruna.Core.Authentication.ApiKey;
using Mistruna.Core.Authorization.Plans;
using Mistruna.Core.RateLimiting.Tiered;
using Xunit;

namespace Mistruna.Core.Tests.RateLimiting.Tiered;

public class TieredRateLimitMiddlewareTests
{
    [Fact]
    public async Task Invoke_UnderQuota_CallsNext_AndSetsHeaders()
    {
        var store = new InMemoryQuotaStore();
        var options = new TieredRateLimitOptions { WindowSeconds = 86_400 };
        var (middleware, nextCalled, context) = Build(store, options, quota: 100, apiKeyId: Guid.NewGuid());

        await middleware.InvokeAsync(context);

        nextCalled.Invoked.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        context.Response.Headers["X-RateLimit-Limit"].ToString().Should().Be("100");
        context.Response.Headers["X-RateLimit-Remaining"].ToString().Should().Be("99");
    }

    [Fact]
    public async Task Invoke_OverQuota_Returns429_AndDoesNotCallNext()
    {
        var store = new InMemoryQuotaStore();
        var keyId = Guid.NewGuid();
        var options = new TieredRateLimitOptions { WindowSeconds = 86_400 };

        for (var i = 0; i < 100; i++)
        {
            await store.IncrementAsync($"quota:{keyId}:{DateTime.UtcNow:yyyyMMdd}", TimeSpan.FromSeconds(options.WindowSeconds), CancellationToken.None);
        }

        var (middleware, nextCalled, context) = Build(store, options, quota: 100, apiKeyId: keyId);

        await middleware.InvokeAsync(context);

        nextCalled.Invoked.Should().BeFalse();
        context.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
        context.Response.Headers["Retry-After"].ToString().Should().NotBeEmpty();
    }

    [Fact]
    public async Task Invoke_MissingQuotaClaim_CallsNext_WithoutEnforcing()
    {
        var store = new InMemoryQuotaStore();
        var options = new TieredRateLimitOptions { WindowSeconds = 86_400 };
        var nextCalled = new NextSpy();
        var middleware = new TieredRateLimitMiddleware(nextCalled.Next, options, store, NullLogger<TieredRateLimitMiddleware>.Instance);

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity())
        };

        await middleware.InvokeAsync(context);

        nextCalled.Invoked.Should().BeTrue();
        context.Response.Headers.ContainsKey("X-RateLimit-Limit").Should().BeFalse();
    }

    private sealed class NextSpy
    {
        public bool Invoked { get; private set; }
        public Task Next(HttpContext _) { Invoked = true; return Task.CompletedTask; }
    }

    private static (TieredRateLimitMiddleware middleware, NextSpy spy, DefaultHttpContext context) Build(
        IQuotaStore store,
        TieredRateLimitOptions options,
        long quota,
        Guid apiKeyId)
    {
        var spy = new NextSpy();
        var middleware = new TieredRateLimitMiddleware(spy.Next, options, store, NullLogger<TieredRateLimitMiddleware>.Instance);

        var identity = new ClaimsIdentity("ApiKey");
        identity.AddClaim(new Claim(ApiKeyClaimTypes.ApiKeyId, apiKeyId.ToString()));
        identity.AddClaim(new Claim(PlanClaimTypes.Tier, Plan.Developer.ToString()));
        identity.AddClaim(new Claim(PlanClaimTypes.Quota, quota.ToString()));

        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        return (middleware, spy, context);
    }
}
