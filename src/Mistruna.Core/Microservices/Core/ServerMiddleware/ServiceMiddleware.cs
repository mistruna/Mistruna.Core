using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Microservices.Core.JwtAuth;
using Newtonsoft.Json;

namespace Mistruna.Core.Microservices.Core.ServerMiddleware;

/// <summary>
/// Utility helpers for configuring and working with ASP.NET Core microservice servers.
/// </summary>
public static class ServiceMiddleware
{
    /// <summary>
    /// Registers controllers with Newtonsoft.Json serialization configured for indented ISO dates
    /// and camelCase naming, which is the expected format for inter-service communication.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void AddServerControllers(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddControllers().AddNewtonsoftJson((Action<MvcNewtonsoftJsonOptions>)(opt =>
        {
            opt.SerializerSettings.Formatting = Formatting.Indented;
            opt.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            opt.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
            opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            opt.UseCamelCasing(true);
        }));
    }

    /// <summary>
    /// Extracts the current user's ID from the "UserId" claim.
    /// </summary>
    /// <param name="claims">The claims principal to read from.</param>
    /// <returns>The parsed user ID as <see cref="Guid"/>.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the UserId claim is missing.</exception>
    public static Guid GetUserId(this IEnumerable<Claim> claims)
        => Guid.Parse((claims.SingleOrDefault(s => s.Type == "UserId")
                       ?? throw new UnauthorizedAccessException("UserId claim is missing.")).Value);

    /// <summary>
    /// Checks whether the current controller user is in the Administrator role.
    /// </summary>
    /// <param name="controller">The controller base instance.</param>
    /// <returns>True if the user is a root administrator.</returns>
    public static bool IsInRootAdmin(this ControllerBase controller)
        => controller.User.IsInRole(RoleType.Administrator);

    /// <summary>
    /// Reads the configured host address for a named microservice from the "ServicesHosts" configuration section.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="name">The service name key.</param>
    /// <returns>The host URL, or null if not configured.</returns>
    public static string GetMicroserviceHost(this IConfiguration configuration, string name)
        => configuration?.GetSection("ServicesHosts")?[name];
}
