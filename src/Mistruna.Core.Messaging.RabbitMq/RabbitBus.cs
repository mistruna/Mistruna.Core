using Mistruna.Core.Messaging.RabbitMq.Internal;
using RabbitMQ.Client;

namespace Mistruna.Core.Messaging.RabbitMq;

/// <summary>
/// Publishes persistent JSON messages with RabbitMQ.Client 7.
/// </summary>
public sealed class RabbitBus : IMistrunaRabbitBus
{
    private readonly IRabbitPublisherChannel _publisher;

    internal RabbitBus(IRabbitPublisherChannel publisher)
    {
        _publisher = publisher;
    }

    /// <inheritdoc />
    public Task PublishAsync<T>(
        T message,
        string exchange,
        string routingKey,
        CancellationToken cancellationToken = default)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentException.ThrowIfNullOrWhiteSpace(exchange);
        ArgumentException.ThrowIfNullOrWhiteSpace(routingKey);

        var properties = new BasicProperties
        {
            ContentEncoding = "utf-8",
            ContentType = "application/json",
            Persistent = true,
            Type = typeof(T).Name
        };

        return _publisher.PublishAsync(
            exchange,
            routingKey,
            properties,
            RabbitMessageSerializer.Serialize(message),
            cancellationToken);
    }
}
