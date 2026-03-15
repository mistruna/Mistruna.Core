namespace Mistruna.Core.Microservices.RabbitMq.Attributes;

public class RabbitQueryAttribute : Attribute
{
    public string ExchangeName { get; set; }

    public string ExchangeType { get; set; }

    public string RouteKey { get; set; }
}
