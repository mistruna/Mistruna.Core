using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Mistruna.Core.Monetization.Authentication.ApiKey;
using Mistruna.Core.Monetization.Idempotency;
using Moq;
using Xunit;

namespace Mistruna.Core.Tests.Monetization.Idempotency;

public class IdempotencyMiddlewareTests
{
    [Fact]
    public async Task Invoke_Get_BypassesMiddleware()
    {
        var store = new Mock<IIdempotencyStore>();
        var options = new IdempotencyOptions();
        var context = BuildContext("GET", idemKey: "k1", userId: "u");
        var nextCalls = 0;

        var middleware = new IdempotencyMiddleware(_ => { nextCalls++; return Task.CompletedTask; }, options, store.Object, NullLogger<IdempotencyMiddleware>.Instance);
        await middleware.InvokeAsync(context);

        nextCalls.Should().Be(1);
        store.Verify(s => s.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_PostWithoutKey_BypassesMiddleware()
    {
        var store = new Mock<IIdempotencyStore>();
        var context = BuildContext("POST", idemKey: null, userId: "u");
        var nextCalls = 0;

        var middleware = new IdempotencyMiddleware(_ => { nextCalls++; return Task.CompletedTask; }, new IdempotencyOptions(), store.Object, NullLogger<IdempotencyMiddleware>.Instance);
        await middleware.InvokeAsync(context);

        nextCalls.Should().Be(1);
        store.Verify(s => s.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_PostWithKey_FirstCall_StoresResponse()
    {
        var store = new Mock<IIdempotencyStore>();
        store.Setup(s => s.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((IdempotentResponse?)null);

        var context = BuildContext("POST", idemKey: "abc", userId: "user-1");
        var middleware = new IdempotencyMiddleware(
            async ctx =>
            {
                ctx.Response.StatusCode = 201;
                ctx.Response.ContentType = "application/json";
                await ctx.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("{\"id\":1}"));
            },
            new IdempotencyOptions(),
            store.Object,
            NullLogger<IdempotencyMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        store.Verify(s => s.SetAsync(
            It.Is<string>(k => k.Contains("user-1") && k.Contains("abc")),
            It.Is<IdempotentResponse>(r => r.StatusCode == 201 && r.ContentType == "application/json"),
            It.IsAny<TimeSpan>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Invoke_PostWithKey_RepeatCall_ReplaysStoredResponse_WithoutCallingNext()
    {
        var store = new Mock<IIdempotencyStore>();
        var stored = new IdempotentResponse(200, "application/json", Encoding.UTF8.GetBytes("{\"cached\":true}"));
        store.Setup(s => s.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(stored);

        var context = BuildContext("POST", idemKey: "abc", userId: "user-1");
        context.Response.Body = new MemoryStream();

        var nextCalls = 0;
        var middleware = new IdempotencyMiddleware(_ => { nextCalls++; return Task.CompletedTask; }, new IdempotencyOptions(), store.Object, NullLogger<IdempotencyMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        nextCalls.Should().Be(0);
        context.Response.StatusCode.Should().Be(200);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();
        body.Should().Be("{\"cached\":true}");
    }

    private static DefaultHttpContext BuildContext(string method, string? idemKey, string userId)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = method;
        if (idemKey is not null)
            context.Request.Headers["Idempotency-Key"] = idemKey;

        var identity = new ClaimsIdentity("ApiKey");
        identity.AddClaim(new Claim(ApiKeyClaimTypes.Subject, userId));
        context.User = new ClaimsPrincipal(identity);

        context.Response.Body = new MemoryStream();
        return context;
    }
}
