using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mistruna.Core.Messaging.RabbitMq.Internal;
using RabbitMQ.Client;

namespace Mistruna.Core.Messaging.RabbitMq.DependencyInjection;

/// <summary>
/// Registers Mistruna RabbitMQ publishing and consumers.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the RabbitMQ bus using the <c>Mistruna:RabbitMq</c> configuration section.
    /// The legacy <c>RabbitMq</c> section is also accepted.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMistrunaRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection("Mistruna:RabbitMq");
        if (!section.Exists())
        {
            section = configuration.GetSection("RabbitMq");
        }

        services.AddOptions<RabbitMqOptions>().Bind(section);
        services.TryAddSingleton<IRabbitPublisherChannel, RabbitPublisherChannel>();
        services.TryAddSingleton<IMistrunaRabbitBus>(
            provider => new RabbitBus(provider.GetRequiredService<IRabbitPublisherChannel>()));
        return services;
    }

    /// <summary>
    /// Registers a typed asynchronous RabbitMQ consumer.
    /// </summary>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <typeparam name="THandler">The scoped handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="queue">The queue to declare and consume.</param>
    /// <param name="exchange">The exchange to declare and bind.</param>
    /// <param name="routingKey">The queue binding routing key.</param>
    /// <param name="exchangeType">The RabbitMQ exchange type.</param>
    /// <param name="durable">Whether the exchange and queue survive broker restarts.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMistrunaRabbitConsumer<TMessage, THandler>(
        this IServiceCollection services,
        string queue,
        string exchange,
        string routingKey,
        string exchangeType = ExchangeType.Topic,
        bool durable = true)
        where TMessage : class
        where THandler : class, IMistrunaRabbitMessageHandler<TMessage>
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(queue);
        ArgumentException.ThrowIfNullOrWhiteSpace(exchange);
        ArgumentException.ThrowIfNullOrWhiteSpace(routingKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(exchangeType);

        services.AddScoped<THandler>();
        services.AddScoped<IMistrunaRabbitMessageHandler<TMessage>>(
            provider => provider.GetRequiredService<THandler>());
        services.AddSingleton(
            new RabbitConsumerRegistration<TMessage, THandler>(
                queue,
                exchange,
                routingKey,
                exchangeType,
                durable));
        services.AddHostedService<RabbitConsumerHostedService<TMessage, THandler>>();
        return services;
    }
}
