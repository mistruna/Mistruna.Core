namespace Mistruna.Core.Microservices.RabbitMq.Attributes;

/// <summary>
/// Marks a message type for RabbitMQ routing, specifying the target exchange and routing key.
/// Used by <see cref="RabbitManager.Send{T}"/> and <see cref="RabbitManager.Consume{T,TE}"/>.
/// </summary>
public class RabbitQueryAttribute : Attribute
{
    /// <summary>The exchange name to publish to or consume from.</summary>
    public string ExchangeName { get; set; }

    /// <summary>The exchange type (e.g., "direct", "topic", "fanout").</summary>
    public string ExchangeType { get; set; }

    /// <summary>The routing key for message delivery.</summary>
    public string RouteKey { get; set; }
}
