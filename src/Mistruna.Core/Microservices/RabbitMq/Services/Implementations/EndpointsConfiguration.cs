using Mistruna.Core.Microservices.RabbitMq.Services.Interfaces;

namespace Mistruna.Core.Microservices.RabbitMq.Services.Implementations;

public class EndpointsConfiguration : IEndpointsConfiguration
{
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
