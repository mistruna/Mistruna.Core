using Microsoft.Extensions.Options;
using Mistruna.Core.Contracts.Microservices.RabbitMq.Services.Interfaces;
using Mistruna.Core.Microservices.RabbitMq.Configurations;

namespace Mistruna.Core.Microservices.RabbitMq.Services.Interfaces;

public interface IEndpointConfiguration
{
    void WithBinding(string exchange, string routingKey);

    IBus BuildWrapper(IServiceProvider services, IOptions<RabbitMqConfiguration> options);
}
