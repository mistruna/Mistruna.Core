# Samples

Sample projects demonstrating Mistruna.Core usage.

## BasicApi

Demonstrates:

- `AddMistrunaCore()` — MediatR + FluentValidation + pipeline behaviors
- CQRS markers — `IQuery<T>` and `ICommand<T>`
- `UseMistrunaExceptionHandler()` — ProblemDetails exception handling
- `AddMistrunaHealthChecks()` — `/health` endpoint
- Swagger in Development

### Run

```bash
dotnet run --project samples/Mistruna.Core.Samples.BasicApi
```

Try:

- `GET /ping/World`
- `POST /counter/increment?step=2`
- `GET /errors/not-found`

## WorkerRabbit

Demonstrates:

- `AddMistrunaRabbitMq()` — binds `Mistruna:RabbitMq` configuration
- `AddMistrunaRabbitConsumer<TMessage, THandler>()` — typed message handling
- Broker-free startup by default

### Run

```bash
dotnet run --project samples/Mistruna.Core.Samples.WorkerRabbit
```

The default configuration binds RabbitMQ options without opening a connection. To
start the consumer, provide a reachable broker and set
`Mistruna__RabbitMq__EnableConsumer=true`. Other settings use the same environment
variable pattern, for example `Mistruna__RabbitMq__HostName`.

## ApiWithOtel

Demonstrates:

- `AddMistrunaObservability()` — ASP.NET Core, HTTP client, and MediatR instrumentation
- `AddMistrunaResilience()` — shared resilience registrations
- `AddMistrunaResilienceHandler()` — a resilient outbound HTTP client

### Run

```bash
dotnet run --project samples/Mistruna.Core.Samples.ApiWithOtel
```

Try:

- `GET /`
- `GET /upstream`

The sample produces OpenTelemetry telemetry. Add the exporter package and exporter
configuration required by your deployment, such as OTLP, in the host application.
