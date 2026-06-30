using Microsoft.Extensions.Options;
using Mistruna.Core.Contracts.Microservices.RabbitMq.Services.Interfaces;
using Mistruna.Core.Microservices.RabbitMq.Configurations;

namespace Mistruna.Core.Microservices.RabbitMq.Services.Interfaces;

/// <summary>
/// Configures a single RabbitMQ endpoint (queue + exchange binding).
/// </summary>
public interface IEndpointConfiguration
{
    /// <summary>
    /// Binds the queue to an exchange with the given routing key.
    /// </summary>
    void WithBinding(string exchange, string routingKey);

    /// <summary>
    /// Builds the message bus wrapper that handles consuming messages from this endpoint.
    /// </summary>
    IBus BuildWrapper(IServiceProvider services, IOptions<RabbitMqConfiguration> options);
}
