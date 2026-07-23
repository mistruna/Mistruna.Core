using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.AspNetCore.ExceptionHandling;

namespace Mistruna.Core.AspNetCore.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMistrunaAspNetCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddExceptionHandler<MistrunaExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
