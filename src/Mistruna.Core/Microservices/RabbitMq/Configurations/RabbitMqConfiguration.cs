namespace Mistruna.Core.Microservices.RabbitMq.Configurations;

/// <summary>
/// Configuration options for connecting to a RabbitMQ broker.
/// </summary>
public class RabbitMqConfiguration
{
    /// <summary>RabbitMQ server hostname.</summary>
    public string Hostname { get; init; }

    /// <summary>Default queue name.</summary>
    public string QueueName { get; init; }

    /// <summary>Authentication username.</summary>
    public string UserName { get; init; }

    /// <summary>Authentication password.</summary>
    public string Password { get; init; }

    /// <summary>RabbitMQ server port. Use -1 for the default AMQP port (5672).</summary>
    public int Port { get; init; }

    /// <summary>Virtual host name.</summary>
    public string VHost { get; init; }
}
