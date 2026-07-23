namespace Mistruna.Core.Messaging.RabbitMq;

/// <summary>
/// Publishes messages through RabbitMQ.
/// </summary>
public interface IMistrunaRabbitBus
{
    /// <summary>
    /// Publishes a message to an exchange using the specified routing key.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="exchange">The destination exchange.</param>
    /// <param name="routingKey">The routing key.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>A task representing the publish operation.</returns>
    Task PublishAsync<T>(
        T message,
        string exchange,
        string routingKey,
        CancellationToken cancellationToken = default)
        where T : class;
}
