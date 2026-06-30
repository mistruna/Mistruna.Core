using System.Text;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mistruna.Core.Contracts.Microservices.RabbitMq.Services.Interfaces;
using Mistruna.Core.Microservices.RabbitMq.Configurations;
using Mistruna.Core.Microservices.RabbitMq.Services.Implementations;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mistruna.Core.Microservices.RabbitMq.RabbitMqEndpointBinder;

/// <summary>
/// Wraps a RabbitMQ queue consumer that dispatches incoming messages through MediatR.
/// Each message is handled within a scoped service provider to support DI in handlers.
/// </summary>
public class RabbitMqWrapper<T> : IBus
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly string _queue;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqWrapper(
        IOptions<RabbitMqConfiguration> options,
        EndpointConfiguration<T> configuration,
        IServiceProvider services)
    {
        _serviceProvider = services;
        _connection = new ConnectionFactory
        {
            HostName = options.Value.Hostname,
            UserName = options.Value.UserName,
            Password = options.Value.Password,
            Port = -1,
            VirtualHost = options.Value.VHost
        }.CreateConnection();
        _channel = _connection.CreateModel();
        _queue = configuration.Queue;
        _channel.QueueDeclare(_queue, configuration.Durable, configuration.Exclusive, configuration.AutoDelete,
            configuration.Arguments);
        _channel.QueueBind(configuration.Queue, configuration.Exchange, configuration.RoutingKey);
    }

    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (EventHandler<BasicDeliverEventArgs>)(async (_, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var message = JsonConvert.DeserializeObject<T>(content);
                _ = await mediator.Send((object)message, stoppingToken);
            }
            finally
            {
                _channel.BasicAck(ea.DeliveryTag, false);
            }
        });
        _channel.BasicConsume(_queue, false, (IBasicConsumer)consumer);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
