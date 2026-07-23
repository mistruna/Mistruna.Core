using RabbitMQ.Client;

namespace Mistruna.Core.Messaging.RabbitMq.Internal;

internal interface IRabbitPublisherChannel
{
    Task PublishAsync(
        string exchange,
        string routingKey,
        BasicProperties properties,
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken);
}
