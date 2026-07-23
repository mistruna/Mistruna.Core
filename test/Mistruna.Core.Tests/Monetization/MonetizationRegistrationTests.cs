using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Monetization.Authorization.Plans;
using Mistruna.Core.Monetization.DependencyInjection;
using Mistruna.Core.Monetization.Idempotency;
using Mistruna.Core.Monetization.Metering;
using Mistruna.Core.Monetization.RateLimiting.Tiered;
using Xunit;

namespace Mistruna.Core.Tests.Monetization;

public sealed class MonetizationRegistrationTests
{
    [Fact]
    public void AddMistrunaApiKeyAuthentication_IsAvailableOnServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddMistrunaApiKeyAuthentication();

        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddMistrunaMonetization_RegistersAllMonetizationServices()
    {
        var services = new ServiceCollection();

        services.AddMistrunaMonetization();

        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(IQuotaStore)
            && descriptor.ImplementationType == typeof(RedisQuotaStore));
        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(IIdempotencyStore)
            && descriptor.ImplementationType == typeof(RedisIdempotencyStore));
        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(IUsageMeter)
            && descriptor.ImplementationType == typeof(RedisUsageMeter));
        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(IAuthorizationPolicyProvider)
            && descriptor.ImplementationType == typeof(RequiresPlanPolicyProvider));
        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(IAuthorizationMiddlewareResultHandler)
            && descriptor.ImplementationType == typeof(PlanAuthorizationMiddlewareResultHandler));
    }

    [Fact]
    public void UseMistrunaMonetization_AddsTheMonetizationPipeline()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMistrunaMonetization();
        using var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);

        var result = app.UseMistrunaMonetization();

        result.Should().BeSameAs(app);
        app.Build().Should().NotBeNull();
    }
}
