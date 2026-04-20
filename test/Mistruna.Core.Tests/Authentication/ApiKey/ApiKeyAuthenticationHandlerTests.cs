using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Mistruna.Core.Authentication.ApiKey;
using Mistruna.Core.Authorization.Plans;
using Moq;
using Xunit;

namespace Mistruna.Core.Tests.Authentication.ApiKey;

public class ApiKeyAuthenticationHandlerTests
{
    [Fact]
    public async Task Authenticate_WithValidHeader_Succeeds_AndPopulatesClaims()
    {
        var userId = Guid.NewGuid();
        var apiKeyId = Guid.NewGuid();
        var validator = new Mock<IApiKeyValidator>();
        validator.Setup(v => v.ValidateAsync("mk_live_abc", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiKeyValidationResult.Success(userId, apiKeyId, Plan.Pro, 100_000));

        var handler = await CreateHandler(validator.Object, headers: new() { ["X-Api-Key"] = "mk_live_abc" });

        var result = await handler.AuthenticateAsync();

        result.Succeeded.Should().BeTrue();
        result.Principal!.FindFirst(ApiKeyClaimTypes.Subject)!.Value.Should().Be(userId.ToString());
        result.Principal.FindFirst(ApiKeyClaimTypes.ApiKeyId)!.Value.Should().Be(apiKeyId.ToString());
        result.Principal.FindFirst(PlanClaimTypes.Tier)!.Value.Should().Be("Pro");
        result.Principal.FindFirst(PlanClaimTypes.Quota)!.Value.Should().Be("100000");
    }

    [Fact]
    public async Task Authenticate_WithMissingHeader_ReturnsNoResult()
    {
        var validator = new Mock<IApiKeyValidator>();
        var handler = await CreateHandler(validator.Object, headers: new());

        var result = await handler.AuthenticateAsync();

        result.None.Should().BeTrue();
        validator.Verify(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Authenticate_WithInvalidKey_Fails()
    {
        var validator = new Mock<IApiKeyValidator>();
        validator.Setup(v => v.ValidateAsync("bad", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiKeyValidationResult.Failure("revoked"));

        var handler = await CreateHandler(validator.Object, headers: new() { ["X-Api-Key"] = "bad" });

        var result = await handler.AuthenticateAsync();

        result.Succeeded.Should().BeFalse();
        result.Failure!.Message.Should().Contain("revoked");
    }

    [Fact]
    public async Task Authenticate_WithQueryParam_WhenAllowed_Succeeds()
    {
        var validator = new Mock<IApiKeyValidator>();
        validator.Setup(v => v.ValidateAsync("mk_live_qp", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiKeyValidationResult.Success(Guid.NewGuid(), Guid.NewGuid(), Plan.Free, 100));

        var handler = await CreateHandler(
            validator.Object,
            headers: new(),
            query: "?access_token=mk_live_qp",
            configure: o => o.AllowQueryParameter = true);

        var result = await handler.AuthenticateAsync();

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task Authenticate_WithQueryParam_WhenNotAllowed_ReturnsNoResult()
    {
        var validator = new Mock<IApiKeyValidator>();
        var handler = await CreateHandler(
            validator.Object,
            headers: new(),
            query: "?access_token=mk_live_qp");

        var result = await handler.AuthenticateAsync();

        result.None.Should().BeTrue();
    }

    private static async Task<ApiKeyAuthenticationHandler> CreateHandler(
        IApiKeyValidator validator,
        Dictionary<string, string> headers,
        string? query = null,
        Action<ApiKeyAuthenticationOptions>? configure = null)
    {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection()
            .AddSingleton(validator)
            .BuildServiceProvider();

        var context = new DefaultHttpContext { RequestServices = services };
        foreach (var h in headers) context.Request.Headers[h.Key] = h.Value;
        if (query is not null) context.Request.QueryString = new QueryString(query);

        var options = new ApiKeyAuthenticationOptions();
        configure?.Invoke(options);

        var optionsMonitor = new Mock<IOptionsMonitor<ApiKeyAuthenticationOptions>>();
        optionsMonitor.Setup(m => m.Get(It.IsAny<string>())).Returns(options);
        optionsMonitor.Setup(m => m.CurrentValue).Returns(options);

        var loggerFactory = NullLoggerFactory.Instance;
        var encoder = UrlEncoder.Default;

        var handler = new ApiKeyAuthenticationHandler(optionsMonitor.Object, loggerFactory, encoder);
        await handler.InitializeAsync(
            new AuthenticationScheme(ApiKeyDefaults.AuthenticationScheme, null, typeof(ApiKeyAuthenticationHandler)),
            context);
        return handler;
    }
}
