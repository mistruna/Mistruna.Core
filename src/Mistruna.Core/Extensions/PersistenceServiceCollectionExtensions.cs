using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Base.Persistence;
using Mistruna.Core.Contracts.Base.Infrastructure;

namespace Mistruna.Core.Extensions;

/// <summary>
/// Extension methods for registering persistence infrastructure services.
/// </summary>
public static class PersistenceServiceCollectionExtensions
{
    /// <summary>
    /// Registers the EF Core unit of work for the given DbContext.
    /// </summary>
    /// <typeparam name="TContext">The EF Core DbContext type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddEfUnitOfWork<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.TryAddScoped<IUnitOfWork, EfUnitOfWork<TContext>>();
        return services;
    }
}
