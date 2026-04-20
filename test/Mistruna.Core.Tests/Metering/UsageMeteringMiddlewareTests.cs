using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Mistruna.Core.Authentication.ApiKey;
using Mistruna.Core.Metering;
using Moq;
using Xunit;

namespace Mistruna.Core.Tests.Metering;

public class UsageMeteringMiddlewareTests
{
    [Fact]
    public async Task Invoke_With2xx_AndApiKeyClaim_IncrementsMeter()
    {
        var meter = new Mock<IUsageMeter>();
        var keyId = Guid.NewGuid();
        var context = BuildContext(apiKeyId: keyId);
        var middleware = new UsageMeteringMiddleware(
            ctx => { ctx.Response.StatusCode = 200; return Task.CompletedTask; },
            new UsageMeteringOptions(),
            meter.Object,
            NullLogger<UsageMeteringMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        meter.Verify(m => m.IncrementAsync(keyId, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Invoke_With4xx_DoesNotIncrementMeter()
    {
        var meter = new Mock<IUsageMeter>();
        var context = BuildContext(apiKeyId: Guid.NewGuid());
        var middleware = new UsageMeteringMiddleware(
            ctx => { ctx.Response.StatusCode = 429; return Task.CompletedTask; },
            new UsageMeteringOptions(),
            meter.Object,
            NullLogger<UsageMeteringMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        meter.Verify(m => m.IncrementAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_WithoutApiKeyClaim_DoesNotIncrementMeter()
    {
        var meter = new Mock<IUsageMeter>();
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) };
        var middleware = new UsageMeteringMiddleware(
            ctx => { ctx.Response.StatusCode = 200; return Task.CompletedTask; },
            new UsageMeteringOptions(),
            meter.Object,
            NullLogger<UsageMeteringMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        meter.Verify(m => m.IncrementAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_WhenMeterThrows_DoesNotPropagate()
    {
        var meter = new Mock<IUsageMeter>();
        meter.Setup(m => m.IncrementAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
             .ThrowsAsync(new InvalidOperationException("Redis down"));

        var context = BuildContext(apiKeyId: Guid.NewGuid());
        var middleware = new UsageMeteringMiddleware(
            ctx => { ctx.Response.StatusCode = 200; return Task.CompletedTask; },
            new UsageMeteringOptions(),
            meter.Object,
            NullLogger<UsageMeteringMiddleware>.Instance);

        Func<Task> act = () => middleware.InvokeAsync(context);

        await act.Should().NotThrowAsync();
    }

    private static DefaultHttpContext BuildContext(Guid apiKeyId)
    {
        var identity = new ClaimsIdentity("ApiKey");
        identity.AddClaim(new Claim(ApiKeyClaimTypes.ApiKeyId, apiKeyId.ToString()));
        return new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
    }
}
