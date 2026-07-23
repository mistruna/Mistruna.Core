using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Abstractions.Persistence;

namespace Mistruna.Core.EfCore;

/// <summary>Registers Mistruna EF Core persistence services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers the generic repository and unit of work for a DbContext.</summary>
    public static IServiceCollection AddMistrunaEfCore<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<DbContext>(
            serviceProvider => serviceProvider.GetRequiredService<TContext>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork<TContext>>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(EfGenericRepository<>));
        return services;
    }
}
