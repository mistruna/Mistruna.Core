# Mistruna.Core

[![CI](https://github.com/mistruna/Mistruna.Core/workflows/CI/badge.svg)](https://github.com/mistruna/Mistruna.Core/actions?query=workflow%3ACI)
[![NuGet](https://img.shields.io/nuget/v/Mistruna.Core.svg)](https://www.nuget.org/packages/Mistruna.Core)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Production-oriented .NET building blocks for ASP.NET Core microservices: CQRS, validation, ProblemDetails, persistence, Redis, RabbitMQ, observability, resilience, and optional monetization primitives.

## Packages

| Package | TFM | Responsibility |
|---------|-----|----------------|
| `Mistruna.Core.Abstractions` | `net8.0;net10.0` | Results, CQRS markers, entity/value-object bases, persistence contracts, domain events, and pagination DTOs |
| `Mistruna.Core` | `net10.0` | MediatR registration, validation and logging behaviors, exceptions, and core DI |
| `Mistruna.Core.AspNetCore` | `net10.0` | ProblemDetails, health, versioning, CORS, JWT, and IP rate limiting |
| `Mistruna.Core.EfCore` | `net10.0` | EF repositories, unit of work, and conventions |
| `Mistruna.Core.Caching.Redis` | `net10.0` | Redis caching and health check |
| `Mistruna.Core.Messaging.RabbitMq` | `net10.0` | RabbitMQ.Client 7 asynchronous bus |
| `Mistruna.Core.Monetization` | `net10.0` | API keys, plan authorization, tiered rate limits, idempotency, and metering |
| `Mistruna.Core.Observability` | `net10.0` | OpenTelemetry registration and MediatR activity enrichment |
| `Mistruna.Core.Resilience` | `net10.0` | HttpClient resilience presets and marked-command retry behavior |
| `Mistruna.Core.Testing` | `net10.0` | EF async providers and consumer test helpers |

There is no umbrella meta-package. Add only the packages your service uses:

```bash
dotnet add package Mistruna.Core
dotnet add package Mistruna.Core.AspNetCore
dotnet add package Mistruna.Core.EfCore
```

## Quick start

```csharp
using Mistruna.Core.DependencyInjection;
using Mistruna.Core.AspNetCore.DependencyInjection;
using Mistruna.Core.EfCore;

builder.Services.AddMistrunaCore(options =>
{
    options.RegisterAssemblies(typeof(Program).Assembly);
    options.AddValidation();
    options.AddLoggingBehavior();
});
builder.Services.AddMistrunaAspNetCore();
builder.Services.AddMistrunaEfCore<AppDbContext>();

var app = builder.Build();
app.UseMistrunaExceptionHandler();
```

Optional integrations compose independently:

```csharp
builder.Services.AddMistrunaRedis(builder.Configuration);
builder.Services.AddMistrunaRabbitMq(builder.Configuration);
builder.Services.AddMistrunaObservability(builder.Configuration);
builder.Services.AddMistrunaResilience();
builder.Services.AddMistrunaMonetization();

app.UseMistrunaTieredRateLimiting();
app.UseMistrunaIdempotency();
app.UseMistrunaUsageMetering();
```

Define requests with the markers from `Mistruna.Core.Abstractions`:

```csharp
public sealed record GetUserQuery(Guid Id) : IQuery<UserDto>;
public sealed record CreateUserCommand(string Email) : ICommand<Guid>;
```

## Foundation features

- System.Text.Json-only serialization and RFC 7807 ProblemDetails
- MediatR command/query markers with opt-in validation and logging behaviors
- EF Core repository, specification, and unit-of-work implementations
- Redis caching and health checks
- RabbitMQ.Client 7 async publishing via `IMistrunaRabbitBus.PublishAsync`
- OpenTelemetry tracing, metrics, and MediatR activity enrichment
- HttpClient resilience presets and optional marked-command retries
- API key authentication, plan authorization, rate limits, idempotency, and metering
- EF async query providers and result assertions for consumer tests

## Samples

```powershell
dotnet run --project samples/Mistruna.Core.Samples.BasicApi
dotnet run --project samples/Mistruna.Core.Samples.WorkerRabbit
dotnet run --project samples/Mistruna.Core.Samples.ApiWithOtel
```

The RabbitMQ sample requires a broker. See [samples/README.md](samples/README.md) for configuration.

## Build

Prerequisite: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

```powershell
dotnet format --verify-no-changes
dotnet test
dotnet pack --configuration Release --output ./artifacts
```

`./Build.ps1` runs restore, build, test, and pack. Packages are written to `artifacts/`.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). Pull requests are welcome.

## License

MIT — see [LICENSE](LICENSE).
