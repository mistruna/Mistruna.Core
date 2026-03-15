namespace Mistruna.Core.Contracts.Microservices.RabbitMq.Services.Interfaces;

/// <summary>
/// Defines a message bus interface for asynchronous message processing.
/// </summary>
public interface IBus
{
    /// <summary>
    /// Executes the message bus processing loop asynchronously.
    /// </summary>
    /// <param name="stoppingToken">A cancellation token to signal when to stop processing.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync(CancellationToken stoppingToken);
}
