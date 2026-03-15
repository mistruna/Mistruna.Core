namespace Mistruna.Core.Microservices.RabbitMq.Services.Interfaces;

public interface IEndpointsConfiguration
{
    List<IEndpointConfiguration> Endpoints { get; set; }
}
