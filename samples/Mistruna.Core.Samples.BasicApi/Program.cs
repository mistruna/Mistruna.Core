using MediatR;
using Mistruna.Core.AspNetCore.DependencyInjection;
using Mistruna.Core.AspNetCore.HealthChecks;
using Mistruna.Core.DependencyInjection;
using Mistruna.Core.Samples.BasicApi.Features.Counter;
using Mistruna.Core.Samples.BasicApi.Features.Ping;

namespace Mistruna.Core.Samples.BasicApi;

/// <summary>
/// Minimal API demonstrating Mistruna.Core registration, CQRS markers, and middleware.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMistrunaCore(options =>
        {
            options.RegisterAssemblies(typeof(Program).Assembly);
            options.AddValidation();
            options.AddLoggingBehavior();
        });
        builder.Services.AddMistrunaAspNetCore();
        builder.Services.AddMistrunaHealthChecks();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseMistrunaExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapHealthChecks("/health");

        app.MapGet("/", () => Results.Ok(new
        {
            message = "Mistruna.Core sample API",
            endpoints = new[] { "/ping/{name}", "/counter/increment", "/errors/not-found" }
        }));

        app.MapGet("/ping/{name}", async (string name, IMediator mediator) =>
            Results.Ok(await mediator.Send(new PingQuery(name))));

        app.MapPost("/counter/increment", async (IMediator mediator, int step = 1) =>
            Results.Ok(await mediator.Send(new IncrementCounterCommand { Step = step })));

        app.MapGet("/errors/not-found", () =>
        {
            throw new Exceptions.NotFoundException("Sample resource not found", "X.NotFound");
        });

        app.Run();
    }
}
