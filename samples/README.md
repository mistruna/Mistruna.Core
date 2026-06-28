# Samples

Sample projects demonstrating Mistruna.Core usage.

## BasicApi

Demonstrates:

- `AddCore()` — MediatR + FluentValidation + pipeline behaviors
- CQRS markers — `IQuery<T>` and `ICommand<T>`
- `UseCoreMiddlewares()` — centralized exception handling
- `AddCoreHealthChecks()` — `/health` endpoint
- Swagger in Development

### Run

```bash
dotnet run --project samples/Mistruna.Core.Samples.BasicApi
```

Try:

- `GET /ping/World`
- `POST /counter/increment?step=2`
- `GET /errors/not-found`
