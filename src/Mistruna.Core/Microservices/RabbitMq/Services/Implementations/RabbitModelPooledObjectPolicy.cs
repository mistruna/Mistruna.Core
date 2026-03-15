using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Mistruna.Core.Microservices.RabbitMq.Configurations;
using RabbitMQ.Client;

namespace Mistruna.Core.Microservices.RabbitMq.Services.Implementations;

public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
{
    private readonly RabbitMqConfiguration _options;
    private readonly IConnection _connection;

    public RabbitModelPooledObjectPolicy(IOptions<RabbitMqConfiguration> options)
    {
        _options = options.Value;
        _connection = GetConnection();
    }

    private IConnection GetConnection()
    {
        return new ConnectionFactory
        {
            HostName = _options.Hostname,
            UserName = _options.UserName,
            Password = _options.Password,
            Port = -1,
            VirtualHost = _options.VHost
        }.CreateConnection();
    }

    public IModel Create() => _connection.CreateModel();

    public bool Return(IModel obj)
    {
        if (obj.IsOpen)
        {
            return true;
        }

        obj.Dispose();

        return false;
    }
}
