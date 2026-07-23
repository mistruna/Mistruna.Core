using Mistruna.Core.DependencyInjection;
using Mistruna.Core.Observability.DependencyInjection;
using Mistruna.Core.Resilience.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMistrunaCore(options =>
    options.RegisterAssemblies(typeof(Program).Assembly));
builder.Services.AddMistrunaObservability(builder.Configuration);
builder.Services.AddMistrunaResilience();
builder.Services
    .AddHttpClient("upstream", client =>
        client.BaseAddress = new Uri("https://example.com"))
    .AddMistrunaResilienceHandler();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    message = "Mistruna observability and resilience sample",
    upstreamEndpoint = "/upstream"
}));

app.MapGet("/upstream", async (
    IHttpClientFactory httpClientFactory,
    CancellationToken cancellationToken) =>
{
    var client = httpClientFactory.CreateClient("upstream");
    var content = await client.GetStringAsync("/", cancellationToken);
    return Results.Text(content, "text/html");
});

await app.RunAsync();
