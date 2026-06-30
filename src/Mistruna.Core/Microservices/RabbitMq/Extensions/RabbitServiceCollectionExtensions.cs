using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Mistruna.Core.Contracts.Microservices.RabbitMq.Services.Interfaces;
using Mistruna.Core.Microservices.RabbitMq.Configurations;
using Mistruna.Core.Microservices.RabbitMq.Services.Implementations;
using Mistruna.Core.Microservices.RabbitMq.Services.Interfaces;
using RabbitMQ.Client;

namespace Mistruna.Core.Microservices.RabbitMq.Extensions;

/// <summary>
/// DI registration helpers for RabbitMQ messaging infrastructure.
/// </summary>
public static class RabbitServiceCollectionExtensions
{
    /// <summary>
    /// Registers the RabbitMQ connection, channel pool, and <see cref="IRabbitManager"/> implementation.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration (expects a "RabbitMq" section).</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddRabbit(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMq"));
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();
        services.AddSingleton<IRabbitManager, RabbitManager>();

        return services;
    }

    /// <summary>
    /// Registers message bus endpoints and the hosted service that starts consuming messages.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Action to configure the endpoints collection.</param>
    public static void AddRabbitMqEndpoints(
        this IServiceCollection services,
        Action<EndpointsConfiguration> configuration)
    {
        var implementationInstance = new EndpointsConfiguration();
        configuration(implementationInstance);
        services.AddSingleton<IEndpointsConfiguration>((IEndpointsConfiguration) implementationInstance);
        services.AddHostedService<RabbitMqHostedService>();
    }
}
