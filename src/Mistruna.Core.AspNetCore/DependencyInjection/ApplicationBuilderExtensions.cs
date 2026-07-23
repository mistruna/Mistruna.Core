using Microsoft.AspNetCore.Builder;

namespace Mistruna.Core.AspNetCore.DependencyInjection;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMistrunaExceptionHandler(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseExceptionHandler();
    }
}
