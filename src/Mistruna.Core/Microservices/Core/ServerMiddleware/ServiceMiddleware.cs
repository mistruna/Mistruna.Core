using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Microservices.Core.JwtAuth.Entities;
using Newtonsoft.Json;

namespace Mistruna.Core.Microservices.Core.ServerMiddleware;

public static class ServiceMiddleware
{
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

    public static Guid GetUserId(this IEnumerable<Claim> claims)
        => Guid.Parse((claims.SingleOrDefault((Func<Claim, bool>)(s => s.Type == "UserId"))
                       ?? throw new UnauthorizedAccessException()).Value);

    public static bool IsInRootAdmin(this ControllerBase controller)
        => controller.User.IsInRole(RoleType.Administrator);

    public static string GetMicroserviceHost(this IConfiguration configuration, string name)
        => configuration?.GetSection("ServicesHosts")?[name];
}
