namespace Mistruna.Core.Messaging.RabbitMq;

/// <summary>
/// Configures the RabbitMQ broker connection.
/// </summary>
public sealed class RabbitMqOptions
{
    /// <summary>Gets or sets the broker host name.</summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>Gets or sets the broker port.</summary>
    public int Port { get; set; } = 5672;

    /// <summary>Gets or sets the broker user name.</summary>
    public string UserName { get; set; } = "guest";

    /// <summary>Gets or sets the broker password.</summary>
    public string Password { get; set; } = "guest";

    /// <summary>Gets or sets the virtual host.</summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>Gets or sets the client-provided connection name.</summary>
    public string ClientProvidedName { get; set; } = "Mistruna";
}
