using Microsoft.Extensions.Options;
using Mistruna.Core.Messaging.RabbitMq;

namespace Mistruna.Core.Samples.WorkerRabbit;

public sealed record OrderSubmitted(Guid OrderId, DateTimeOffset SubmittedAt);

public sealed class OrderSubmittedHandler(ILogger<OrderSubmittedHandler> logger)
    : IMistrunaRabbitMessageHandler<OrderSubmitted>
{
    public Task HandleAsync(
        OrderSubmitted message,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Received order {OrderId} submitted at {SubmittedAt}.",
            message.OrderId,
            message.SubmittedAt);
        return Task.CompletedTask;
    }
}

public sealed class ConfigurationReadyWorker(
    IOptions<RabbitMqOptions> options,
    ILogger<ConfigurationReadyWorker> logger)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "RabbitMQ configuration loaded for {HostName}:{Port}. Set Mistruna__RabbitMq__EnableConsumer=true to connect.",
            options.Value.HostName,
            options.Value.Port);
        return Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }
}
