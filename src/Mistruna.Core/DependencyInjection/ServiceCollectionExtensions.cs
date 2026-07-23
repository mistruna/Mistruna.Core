using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Behaviors;

namespace Mistruna.Core.DependencyInjection;

/// <summary>Provides Mistruna.Core dependency injection registrations.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds configured Mistruna.Core services.</summary>
    public static IServiceCollection AddMistrunaCore(
        this IServiceCollection services,
        Action<MistrunaCoreOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new MistrunaCoreOptions();
        configure?.Invoke(options);

        if (options.EnableValidation && options.Assemblies.Count == 0)
        {
            throw new InvalidOperationException(
                "AddValidation() requires RegisterAssemblies(...) so validators can be discovered.");
        }

        if (options.Assemblies.Count > 0)
        {
            var assemblies = options.Assemblies.Distinct().ToArray();
            services.AddMediatR(configuration =>
                configuration.RegisterServicesFromAssemblies(assemblies));

            if (options.EnableValidation)
                AddValidators(services, assemblies);
        }

        if (options.EnableLoggingBehavior)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        if (options.EnableValidation)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

        return services;
    }

    private static void AddValidators(IServiceCollection services, params System.Reflection.Assembly[] assemblies)
    {
        var validatorTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes()
                .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition)
                .SelectMany(type => type.GetInterfaces()
                    .Where(@interface =>
                        @interface.IsGenericType &&
                        @interface.GetGenericTypeDefinition() == typeof(IValidator<>))
                    .Select(@interface => new
                    {
                        Interface = @interface,
                        Implementation = type
                    })));

        foreach (var validator in validatorTypes)
            services.AddTransient(validator.Interface, validator.Implementation);
    }
}
