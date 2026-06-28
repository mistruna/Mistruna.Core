# Mistruna.Core

[![CI](https://github.com/mistruna/Mistruna.Core/workflows/CI/badge.svg)](https://github.com/mistruna/Mistruna.Core/actions?query=workflow%3ACI)
[![NuGet](https://img.shields.io/nuget/v/Mistruna.Core.svg)](https://www.nuget.org/packages/Mistruna.Core)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Mistruna.Core.svg)](https://www.nuget.org/packages/Mistruna.Core)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Production-oriented SDK for ASP.NET Core microservices** — MediatR CQRS, validation, JWT, RabbitMQ, Redis, health checks, rate limiting, idempotency, usage metering, and consistent API primitives.

[Quick start](#quick-start) • [Packages](#packages) • [CQRS](#cqrs-icommand--iquery) • [Features](#features) • [Samples](samples/)

---

## Packages

| Package | Target | Purpose |
|---------|--------|---------|
| `Mistruna.Core` | .NET 10 | Full SDK implementation |
| `Mistruna.Core.Contracts` | .NET Standard 2.0 | Interfaces, entities, `Result<T>`, specifications |

```bash
dotnet add package Mistruna.Core
# optional contracts-only package for shared libraries:
dotnet add package Mistruna.Core.Contracts
```

---

## Quick start

```csharp
// Program.cs
builder.Services.AddCore(typeof(Program).Assembly);
builder.Services.AddCoreHealthChecks();

var app = builder.Build();
app.UseCoreMiddlewares();
app.MapHealthChecks("/health");
```

Define handlers with CQRS markers:

```csharp
public sealed record GetUserQuery(Guid Id) : IQuery<UserDto>;

public sealed class CreateUserCommand : ICommand<Guid>
{
    public string Email { get; init; } = string.Empty;
}
```

Run the sample API:

```bash
dotnet run --project samples/Mistruna.Core.Samples.BasicApi
```

---

## CQRS (`ICommand` / `IQuery`)

Mistruna.Core ships MediatR marker interfaces in `Mistruna.Core.Abstractions`:

- **`ICommand` / `ICommand<TResponse>`** — write operations (create, update, delete, auth flows)
- **`IQuery<TResponse>`** — read-only operations

Use `RequestKind` in custom pipeline behaviors:

```csharp
if (!RequestKind.IsCommand(typeof(TRequest)))
    return await next();
```

This keeps audit logging, metering, and side effects off read paths.

---

## Features

### Application core
- MediatR registration (`AddCore`)
- FluentValidation pipeline (`RequestValidationBehavior`)
- Structured request logging (`LoggingBehavior`)
- Centralized exception middleware (`UseCoreMiddlewares`)
- Custom exceptions mapped to HTTP status codes

### Domain & data (Contracts + Core)
- `Result<T>` / `Error` functional errors
- Entity base types (`Entity`, `AuditableEntity`, `SoftDeletableEntity`)
- Value objects (`Email`, `PhoneNumber`, `Money`, `Address`, `DateRange`)
- Specification pattern + `EfGenericRepository` / `EfUnitOfWork`
- Domain event contracts

### Infrastructure
- JWT authentication helpers
- RabbitMQ integration
- Redis caching (`AddRedisCaching`)
- Health checks (`AddCoreHealthChecks`, `AddDatabaseHealthCheck<TDbContext>`, `AddRedisHealthCheck`)
- IP rate limiting (`UseRateLimiting`)

### Monetization primitives (optional, compose as needed)
- API key authentication (`AddMistrunaApiKeyAuthentication`)
- Plan-based authorization with HTTP 402 (`AddMistrunaPlanAuthorization`, `[RequiresPlan]`)
- Tiered rate limiting (`AddMistrunaTieredRateLimiting`, `UseTieredRateLimiting`)
- Idempotency middleware (`AddMistrunaIdempotency`, `UseIdempotency`)
- Usage metering (`AddMistrunaUsageMetering`, `UseUsageMetering`)

### Testing helpers
- `TestAsyncQueryProvider<T>` / `TestAsyncEnumerable<T>` — async EF Core repository tests without a database

---

## Exception handling

`UseCoreMiddlewares()` maps exceptions to HTTP responses:

| Exception | Status |
|-----------|--------|
| `ValidationException` | 400 |
| `NotFoundException` | 404 |
| `UnauthorizedAccessException` | 401 |
| `ForbiddenAccessException` | 403 |
| `ConflictException` | 409 |
| `TimeoutException` | 408 |

---

## Health checks

```csharp
builder.Services
    .AddCoreHealthChecks(builder => builder
        .AddDatabaseHealthCheck<AppDbContext>()
        .AddRedisHealthCheck());
```

---

## Building from source

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

```powershell
./Build.ps1          # build, test, pack
./BuildContracts.ps1 # contracts package only
./Push.ps1 -ApiKey "your-api-key"
```

Place the NuGet icon at `assets/logo/mistruna_128x128.png` before publishing packages (optional for local builds).

---

## Project structure

```
Mistruna.Core/
├── src/
│   ├── Mistruna.Core/              # Main SDK
│   │   ├── Abstractions/           # ICommand, IQuery, RequestKind
│   │   ├── Authentication/         # API key auth
│   │   ├── Authorization/          # Plan-based authorization
│   │   ├── Base/                   # EF repository helpers
│   │   ├── Extensions/             # DI and middleware registration
│   │   ├── Filters/                # MediatR pipeline behaviors
│   │   ├── HealthChecks/           # DB / Redis health checks
│   │   ├── Idempotency/            # Idempotency middleware
│   │   ├── Metering/               # Usage metering
│   │   ├── Microservices/          # JWT, RabbitMQ, Swagger
│   │   ├── Middlewares/            # Exception handling
│   │   ├── Providers/              # EF async test helpers
│   │   └── RateLimiting/           # IP + tiered rate limits
│   └── Mistruna.Core.Contracts/    # Shared contracts
├── test/Mistruna.Core.Tests/
├── samples/Mistruna.Core.Samples.BasicApi/
└── assets/logo/                    # Package icon (add before publish)
```

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). Pull requests are welcome.

## License

MIT — see [LICENSE](LICENSE).
