using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mistruna.Core.Messaging.RabbitMq.Internal;

internal sealed class RabbitConsumerHostedService<TMessage, THandler>(
    IOptions<RabbitMqOptions> options,
    IServiceScopeFactory scopeFactory,
    RabbitConsumerRegistration<TMessage, THandler> registration,
    ILogger<RabbitConsumerHostedService<TMessage, THandler>> logger)
    : BackgroundService
    where TMessage : class
    where THandler : class, IMistrunaRabbitMessageHandler<TMessage>
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = RabbitConnectionFactory.Create(options.Value);
        await using var connection = await factory
            .CreateConnectionAsync(stoppingToken)
            .ConfigureAwait(false);
        await using var channel = await connection
            .CreateChannelAsync(cancellationToken: stoppingToken)
            .ConfigureAwait(false);

        await channel.ExchangeDeclareAsync(
            registration.Exchange,
            registration.ExchangeType,
            registration.Durable,
            autoDelete: false,
            cancellationToken: stoppingToken).ConfigureAwait(false);
        await channel.QueueDeclareAsync(
            registration.Queue,
            registration.Durable,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken).ConfigureAwait(false);
        await channel.QueueBindAsync(
            registration.Queue,
            registration.Exchange,
            registration.RoutingKey,
            cancellationToken: stoppingToken).ConfigureAwait(false);
        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken).ConfigureAwait(false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, delivery) =>
        {
            try
            {
                var message = RabbitMessageSerializer.Deserialize<TMessage>(delivery.Body);
                await using var scope = scopeFactory.CreateAsyncScope();
                var handler = scope.ServiceProvider
                    .GetRequiredService<IMistrunaRabbitMessageHandler<TMessage>>();
                await handler.HandleAsync(message, stoppingToken).ConfigureAwait(false);
                await channel.BasicAckAsync(
                    delivery.DeliveryTag,
                    multiple: false,
                    stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // The host is stopping; the unacknowledged delivery will be requeued by RabbitMQ.
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to handle RabbitMQ message from queue {Queue} with routing key {RoutingKey}.",
                    registration.Queue,
                    registration.RoutingKey);
                await channel.BasicNackAsync(
                    delivery.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    CancellationToken.None).ConfigureAwait(false);
            }
        };

        await channel.BasicConsumeAsync(
            registration.Queue,
            autoAck: false,
            consumer,
            stoppingToken).ConfigureAwait(false);
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken).ConfigureAwait(false);
    }
}
