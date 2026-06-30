namespace Mistruna.Core.Microservices.RabbitMq.Services.Interfaces;

/// <summary>
/// Aggregates multiple <see cref="IEndpointConfiguration"/> instances for batch registration.
/// </summary>
public interface IEndpointsConfiguration
{
    /// <summary>Collection of configured endpoints.</summary>
    List<IEndpointConfiguration> Endpoints { get; set; }
}
