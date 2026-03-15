using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Mistruna.Core.Contracts.Microservices.RabbitMq.Services.Interfaces;
using Mistruna.Core.Microservices.RabbitMq.Configurations;
using Mistruna.Core.Microservices.RabbitMq.Services.Implementations;
using Mistruna.Core.Microservices.RabbitMq.Services.Interfaces;
using RabbitMQ.Client;

namespace Mistruna.Core.Microservices.RabbitMq.Extensions;

public static class RabbitServiceCollectionExtensions
{
    public static IServiceCollection AddRabbit(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMq"));
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();
        services.AddSingleton<IRabbitManager, RabbitManager>();

        return services;
    }

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
