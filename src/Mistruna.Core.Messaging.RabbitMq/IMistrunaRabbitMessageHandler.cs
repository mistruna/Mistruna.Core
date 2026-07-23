namespace Mistruna.Core.Messaging.RabbitMq;

/// <summary>
/// Handles a message delivered by a registered RabbitMQ consumer.
/// </summary>
/// <typeparam name="TMessage">The message type.</typeparam>
public interface IMistrunaRabbitMessageHandler<in TMessage>
    where TMessage : class
{
    /// <summary>
    /// Handles a delivered message.
    /// </summary>
    /// <param name="message">The deserialized message.</param>
    /// <param name="cancellationToken">A token that cancels message processing.</param>
    /// <returns>A task representing the handler operation.</returns>
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}
