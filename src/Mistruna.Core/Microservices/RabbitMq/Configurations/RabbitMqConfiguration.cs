namespace Mistruna.Core.Microservices.RabbitMq.Configurations;

public class RabbitMqConfiguration
{
    public string Hostname { get; init; }

    public string QueueName { get; init; }

    public string UserName { get; init; }

    public string Password { get; init; }

    public int Port { get; init; }

    public string VHost { get; init; }
}
