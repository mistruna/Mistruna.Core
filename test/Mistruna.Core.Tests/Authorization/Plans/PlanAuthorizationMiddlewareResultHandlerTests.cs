using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Mistruna.Core.Authorization.Plans;
using Xunit;

namespace Mistruna.Core.Tests.Authorization.Plans;

public class PlanAuthorizationMiddlewareResultHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenRequiresPlanRequirementFails_WritesStatus402()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new RequiresPlanRequirement(Plan.Pro))
            .Build();
        var authResult = PolicyAuthorizationResult.Forbid(
            AuthorizationFailure.Failed([new RequiresPlanRequirement(Plan.Pro)]));
        var handler = new PlanAuthorizationMiddlewareResultHandler();

        await handler.HandleAsync(_ => Task.CompletedTask, context, policy, authResult);

        context.Response.StatusCode.Should().Be(StatusCodes.Status402PaymentRequired);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Position = 0;
        using var doc = await JsonDocument.ParseAsync(context.Response.Body);
        doc.RootElement.GetProperty("errorCode").GetString().Should().Be("UPGRADE_REQUIRED");
        doc.RootElement.GetProperty("requiredPlan").GetString().Should().Be("Pro");
    }

    [Fact]
    public async Task HandleAsync_WhenUnrelatedFailure_DelegatesToDefault()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        var authResult = PolicyAuthorizationResult.Forbid(
            AuthorizationFailure.Failed((IEnumerable<IAuthorizationRequirement>)[]));
        var handler = new PlanAuthorizationMiddlewareResultHandler();

        await handler.HandleAsync(_ => Task.CompletedTask, context, policy, authResult);

        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }
}
