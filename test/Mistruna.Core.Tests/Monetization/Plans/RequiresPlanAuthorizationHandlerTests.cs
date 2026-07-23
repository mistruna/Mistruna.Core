using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Mistruna.Core.Monetization.Authorization.Plans;
using Xunit;

namespace Mistruna.Core.Tests.Monetization.Plans;

public class RequiresPlanAuthorizationHandlerTests
{
    [Theory]
    [InlineData("Free", Plan.Free, true)]
    [InlineData("Developer", Plan.Free, true)]
    [InlineData("Pro", Plan.Developer, true)]
    [InlineData("Enterprise", Plan.Pro, true)]
    [InlineData("Free", Plan.Developer, false)]
    [InlineData("Developer", Plan.Pro, false)]
    [InlineData("Pro", Plan.Enterprise, false)]
    public async Task HandleRequirement_ComparesTierAgainstRequirement(
        string currentTierClaim, Plan required, bool shouldSucceed)
    {
        var requirement = new RequiresPlanRequirement(required);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(PlanClaimTypes.Tier, currentTierClaim)], "Test"));
        var context = new AuthorizationHandlerContext([requirement], principal, resource: null);
        var handler = new RequiresPlanAuthorizationHandler();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().Be(shouldSucceed);
    }

    [Fact]
    public async Task HandleRequirement_WithMissingTierClaim_Fails()
    {
        var requirement = new RequiresPlanRequirement(Plan.Free);
        var principal = new ClaimsPrincipal(new ClaimsIdentity([], "Test"));
        var context = new AuthorizationHandlerContext([requirement], principal, resource: null);
        var handler = new RequiresPlanAuthorizationHandler();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirement_WithUnparseableTierClaim_Fails()
    {
        var requirement = new RequiresPlanRequirement(Plan.Free);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(PlanClaimTypes.Tier, "SuperDuper")], "Test"));
        var context = new AuthorizationHandlerContext([requirement], principal, resource: null);
        var handler = new RequiresPlanAuthorizationHandler();

        await handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }
}
