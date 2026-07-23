namespace Mistruna.Core.Messaging.RabbitMq.Internal;

internal sealed record RabbitConsumerRegistration<TMessage, THandler>(
    string Queue,
    string Exchange,
    string RoutingKey,
    string ExchangeType,
    bool Durable)
    where TMessage : class
    where THandler : class, IMistrunaRabbitMessageHandler<TMessage>;
