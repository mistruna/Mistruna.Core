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
                IMediator mediator;
                T message;
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    message = JsonConvert.DeserializeObject<T>(content);
                    object obj = await mediator.Send((object)message, stoppingToken);
                }

                mediator = (IMediator)null;
                message = default(T);
            }
            finally
            {
                _channel.BasicAck(ea.DeliveryTag, false);
            }

            content = (string)null;
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
