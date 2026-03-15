using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Filters;

namespace Mistruna.Core.Extensions;

/// <summary>
/// Extension methods for configuring services in the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all Mistruna.Core services including MediatR, validation, and logging behaviors.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">Assemblies to scan for handlers and validators.</param>
    /// <returns>The service collection.</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddCore(typeof(Program).Assembly);
    /// </code>
    /// </example>
    public static IServiceCollection AddCore(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
        services.AddValidators(assemblies);
        services.AddCoreBehaviors();

        return services;
    }

    /// <summary>
    /// Adds all validators from the specified assemblies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">Assemblies to scan for validators.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddValidators(this IServiceCollection services, params Assembly[] assemblies)
    {
        var validatorTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))
                    .Select(i => new { Interface = i, Implementation = t })));

        foreach (var validator in validatorTypes)
        {
            services.AddTransient(validator.Interface, validator.Implementation);
        }

        return services;
    }

    /// <summary>
    /// Adds the standard MediatR pipeline behaviors.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCoreBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

        return services;
    }

    /// <summary>
    /// Adds only the validation behavior without logging.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddValidationBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        return services;
    }

    /// <summary>
    /// Adds a service as a singleton if it doesn't already exist.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection TryAddSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        if (!services.Any(x => x.ServiceType == typeof(TService)))
        {
            services.AddSingleton<TService, TImplementation>();
        }

        return services;
    }

    /// <summary>
    /// Adds a service as scoped if it doesn't already exist.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection TryAddScoped<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        if (!services.Any(x => x.ServiceType == typeof(TService)))
        {
            services.AddScoped<TService, TImplementation>();
        }

        return services;
    }

    /// <summary>
    /// Adds a service as transient if it doesn't already exist.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection TryAddTransient<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        if (!services.Any(x => x.ServiceType == typeof(TService)))
        {
            services.AddTransient<TService, TImplementation>();
        }

        return services;
    }

    /// <summary>
    /// Decorates a service with a decorator.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TDecorator">The decorator type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection Decorate<TService, TDecorator>(this IServiceCollection services)
        where TService : class
        where TDecorator : class, TService
    {
        var wrappedDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(TService))
            ?? throw new InvalidOperationException($"Service {typeof(TService).Name} is not registered.");

        var objectFactory = ActivatorUtilities.CreateFactory(
            typeof(TDecorator),
            new[] { typeof(TService) });

        services.Add(ServiceDescriptor.Describe(
            typeof(TService),
            sp => (TService)objectFactory(sp, new[] { sp.CreateInstance(wrappedDescriptor) }),
            wrappedDescriptor.Lifetime));

        services.Remove(wrappedDescriptor);

        return services;
    }

    private static object CreateInstance(this IServiceProvider services, ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationInstance != null)
            return descriptor.ImplementationInstance;

        if (descriptor.ImplementationFactory != null)
            return descriptor.ImplementationFactory(services);

        return ActivatorUtilities.GetServiceOrCreateInstance(
            services,
            descriptor.ImplementationType!);
    }
}
