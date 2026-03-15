using Microsoft.Extensions.Options;
using Mistruna.Core.Contracts.Microservices.RabbitMq.Services.Interfaces;
using Mistruna.Core.Microservices.RabbitMq.Configurations;
using Mistruna.Core.Microservices.RabbitMq.RabbitMqEndpointBinder;
using Mistruna.Core.Microservices.RabbitMq.Services.Interfaces;

namespace Mistruna.Core.Microservices.RabbitMq.Services.Implementations;

public class EndpointConfiguration<T> : IEndpointConfiguration
{
    public string Queue { get; init; }

    public bool Durable { get; init; }

    public bool Exclusive { get; init; }

    public bool AutoDelete { get; init; }

    public IDictionary<string, object> Arguments { get; init; }

    public string Exchange { get; private set; }

    public string RoutingKey { get; private set; }

    public void WithBinding(string exchange, string routingKey)
    {
        Exchange = exchange;
        RoutingKey = routingKey;
    }

    public IBus BuildWrapper(IServiceProvider services, IOptions<RabbitMqConfiguration> options)
        => new RabbitMqWrapper<T>(options, this, services);
}
