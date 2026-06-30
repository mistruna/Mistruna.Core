using Mistruna.Core.Microservices.RabbitMq.Services.Interfaces;

namespace Mistruna.Core.Microservices.RabbitMq.Services.Implementations;

/// <summary>
/// Default implementation of <see cref="IEndpointsConfiguration"/> that collects endpoint definitions.
/// </summary>
public class EndpointsConfiguration : IEndpointsConfiguration
{
    /// <summary>
    /// Maps a message type to a RabbitMQ queue with optional binding configuration.
    /// </summary>
    /// <typeparam name="T">The message type handled by this endpoint.</typeparam>
    /// <param name="queue">The queue name.</param>
    /// <param name="durable">Whether the queue survives broker restarts.</param>
    /// <param name="exclusive">Whether the queue is limited to this connection.</param>
    /// <param name="autoDelete">Whether the queue is deleted when the last consumer unsubscribes.</param>
    /// <param name="arguments">Optional RabbitMQ arguments (e.g., dead-letter exchange).</param>
    /// <returns>The created endpoint configuration for further chaining.</returns>
    public IEndpointConfiguration MapEndpoint<T>(
        string queue,
        bool durable = true,
        bool exclusive = false,
        bool autoDelete = false,
        IDictionary<string, object> arguments = null)
    {
        var endpointConfiguration = new EndpointConfiguration<T>
        {
            Queue = queue,
            Durable = durable,
            Exclusive = exclusive,
            AutoDelete = autoDelete,
            Arguments = arguments
        };
        Endpoints.Add(endpointConfiguration);

        return endpointConfiguration;
    }

    public List<IEndpointConfiguration> Endpoints { get; set; } = [];
}
