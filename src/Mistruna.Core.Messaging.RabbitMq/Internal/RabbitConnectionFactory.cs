using RabbitMQ.Client;

namespace Mistruna.Core.Messaging.RabbitMq.Internal;

internal static class RabbitConnectionFactory
{
    internal static ConnectionFactory Create(RabbitMqOptions options)
        => new()
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password,
            VirtualHost = options.VirtualHost,
            ClientProvidedName = options.ClientProvidedName
        };
}
