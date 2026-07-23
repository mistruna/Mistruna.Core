using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Mistruna.Core.Messaging.RabbitMq;
using Mistruna.Core.Messaging.RabbitMq.DependencyInjection;
using Mistruna.Core.Messaging.RabbitMq.Internal;
using RabbitMQ.Client;
using Xunit;

namespace Mistruna.Core.Tests.Messaging;

public sealed class RabbitMqTests
{
    [Fact]
    public void Serializer_RoundTrips_Message_Using_SystemTextJson()
    {
        var message = new TestMessage("hello", 42);

        var bytes = RabbitMessageSerializer.Serialize(message);
        var deserialized = RabbitMessageSerializer.Deserialize<TestMessage>(bytes);

        Assert.Equal(message, deserialized);
        Assert.Contains("\"text\":\"hello\"", Encoding.UTF8.GetString(bytes.Span));
    }

    [Fact]
    public async Task PublishAsync_Forwards_Topology_And_Persistent_Json_Message()
    {
        var channel = new RecordingPublisherChannel();
        var bus = new RabbitBus(channel);
        var message = new TestMessage("hello", 42);

        await bus.PublishAsync(message, "events", "test.created", CancellationToken.None);

        Assert.Equal("events", channel.Exchange);
        Assert.Equal("test.created", channel.RoutingKey);
        Assert.True(channel.Properties?.Persistent);
        Assert.Equal(nameof(TestMessage), channel.Properties?.Type);
        Assert.Equal("application/json", channel.Properties?.ContentType);
        Assert.Equal(message, RabbitMessageSerializer.Deserialize<TestMessage>(channel.Body));
    }

    [Fact]
    public async Task AddMistrunaRabbitMq_Registers_Bus_Without_Opening_A_Connection()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Mistruna:RabbitMq:HostName"] = "broker",
                ["Mistruna:RabbitMq:UserName"] = "guest",
                ["Mistruna:RabbitMq:Password"] = "guest"
            })
            .Build();
        var services = new ServiceCollection();

        services.AddMistrunaRabbitMq(configuration);

        await using var provider = services.BuildServiceProvider();
        Assert.IsType<RabbitBus>(provider.GetRequiredService<IMistrunaRabbitBus>());
        Assert.Equal("broker", provider.GetRequiredService<IOptions<RabbitMqOptions>>().Value.HostName);
    }

    [Fact]
    public void AddMistrunaRabbitConsumer_Registers_Handler_And_Hosted_Service()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMistrunaRabbitMq(new ConfigurationBuilder().Build());

        services.AddMistrunaRabbitConsumer<TestMessage, TestHandler>(
            queue: "test-messages",
            exchange: "events",
            routingKey: "test.created");

        using var provider = services.BuildServiceProvider();
        Assert.IsType<TestHandler>(provider.GetRequiredService<IMistrunaRabbitMessageHandler<TestMessage>>());
        Assert.Contains(
            provider.GetServices<IHostedService>(),
            service => service.GetType().IsGenericType
                && service.GetType().GetGenericTypeDefinition() == typeof(RabbitConsumerHostedService<,>));
        var topology = provider.GetRequiredService<RabbitConsumerRegistration<TestMessage, TestHandler>>();
        Assert.Equal("test-messages", topology.Queue);
        Assert.Equal("events", topology.Exchange);
        Assert.Equal("test.created", topology.RoutingKey);
        Assert.Equal(ExchangeType.Topic, topology.ExchangeType);
        Assert.True(topology.Durable);
    }

    private sealed record TestMessage(string Text, int Count);

    private sealed class TestHandler : IMistrunaRabbitMessageHandler<TestMessage>
    {
        public Task HandleAsync(TestMessage message, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class RecordingPublisherChannel : IRabbitPublisherChannel
    {
        public string? Exchange { get; private set; }

        public string? RoutingKey { get; private set; }

        public BasicProperties? Properties { get; private set; }

        public ReadOnlyMemory<byte> Body { get; private set; }

        public Task PublishAsync(
            string exchange,
            string routingKey,
            BasicProperties properties,
            ReadOnlyMemory<byte> body,
            CancellationToken cancellationToken)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            Properties = properties;
            Body = body;
            return Task.CompletedTask;
        }
    }
}
