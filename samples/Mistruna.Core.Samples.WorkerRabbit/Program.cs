using Mistruna.Core.Messaging.RabbitMq.DependencyInjection;
using Mistruna.Core.Samples.WorkerRabbit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMistrunaRabbitMq(builder.Configuration);

if (builder.Configuration.GetValue("Mistruna:RabbitMq:EnableConsumer", false))
{
    builder.Services.AddMistrunaRabbitConsumer<OrderSubmitted, OrderSubmittedHandler>(
        queue: "sample.orders",
        exchange: "sample",
        routingKey: "orders.submitted");
}
else
{
    builder.Services.AddHostedService<ConfigurationReadyWorker>();
}

await builder.Build().RunAsync();
