using Mistruna.Core.Contracts.Base.Infrastructure;

namespace Mistruna.Core.Contracts.Microservices.RabbitMq.Services.Interfaces;

/// <summary>
/// Manages RabbitMQ message operations including publishing, subscribing, and sending messages.
/// </summary>
public interface IRabbitManager
{
    /// <summary>
    /// Publishes a message to a RabbitMQ exchange.
    /// </summary>
    /// <typeparam name="T">The type of message to publish.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="exchangeName">The name of the exchange.</param>
    /// <param name="exchangeType">The type of exchange (e.g., "direct", "topic", "fanout").</param>
    /// <param name="routeKey">The routing key for the message.</param>
    void Publish<T>(T message, string exchangeName, string exchangeType, string routeKey) where T : class;

    /// <summary>
    /// Subscribes to a RabbitMQ queue and retrieves a message of the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type to subscribe for.</typeparam>
    /// <param name="queueName">The name of the queue to subscribe to.</param>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <returns>The retrieved entity.</returns>
    T Subscribe<T>(string queueName, Guid id) where T : IEntity;

    /// <summary>
    /// Sends a message directly to the default exchange.
    /// </summary>
    /// <typeparam name="T">The type of message to send.</typeparam>
    /// <param name="message">The message to send.</param>
    void Send<T>(T message) where T : class;
}
