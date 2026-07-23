# Mistruna.Core Foundation v5 — Design Spec

**Date:** 2026-07-23  
**Status:** Approved (brainstorm)  
**Scope:** Foundation wave only (modular hard-cut rewrite)  
**Out of scope:** Brand/docs site wave, consumer service migration PRs, source generators / deep platform features

---

## 1. Goals

Ship an open-source–grade **v5** of Mistruna.Core that millions of ASP.NET microservice developers can adopt:

- Clear package boundaries (pay only for what you use)
- Modern .NET 10 / System.Text.Json baseline
- Aggressive removal of domain junk and legacy abstractions
- Hard-cut public API with a migration guide
- Essential platform capabilities: OpenTelemetry + HTTP resilience as opt-in packages

Non-goals for this wave: visual rebrand, DocFX/site, migrating `user-service` / `currency-service` in-repo, MassTransit-class broker abstraction, Roslyn analyzers pack.

---

## 2. Decisions (locked)

| Topic | Choice |
|-------|--------|
| First milestone | Foundation (structure, API, deps, tests, DX) |
| Breaking changes | Hard cut (no compatibility shims) |
| Packaging | Thin core + opt-in packages |
| Package IDs | Keep `Mistruna.*` |
| Trim policy | Aggressive — universal microservice primitives only |
| Stack baseline | net10 host packages; Abstractions `net8.0;net10.0`; STJ; RabbitMQ.Client 7.x |
| New capabilities in Foundation | Observability + Resilience opt-in packages |
| Implementation approach | Package-first rewrite (no strangler meta-package) |

---

## 3. Package architecture

### 3.1 Packages

| Package | TFM | Responsibility |
|---------|-----|----------------|
| `Mistruna.Core.Abstractions` | `net8.0;net10.0` | `Result`/`Error`, CQRS markers, entity/VO bases, persistence interfaces, domain events, pagination DTOs |
| `Mistruna.Core` | `net10.0` | MediatR registration, FluentValidation + logging pipeline behaviors, exception types, core DI |
| `Mistruna.Core.AspNetCore` | `net10.0` | ProblemDetails exception handling, health/versioning/CORS/JWT helpers, IP rate limiting |
| `Mistruna.Core.EfCore` | `net10.0` | `EfGenericRepository`, `EfUnitOfWork`, EF conventions helpers |
| `Mistruna.Core.Caching.Redis` | `net10.0` | Redis cache implementation + health check |
| `Mistruna.Core.Messaging.RabbitMq` | `net10.0` | Async RabbitMQ.Client 7 bus (rewrite) |
| `Mistruna.Core.Monetization` | `net10.0` | API key auth, plan authorization (402), tiered rate limits, idempotency, usage metering |
| `Mistruna.Core.Observability` | `net10.0` | OpenTelemetry setup hooks + MediatR Activity enrichment |
| `Mistruna.Core.Resilience` | `net10.0` | HttpClient resilience presets + optional marked-command MediatR retry |
| `Mistruna.Core.Testing` | `net10.0` | EF async test providers and consumer test helpers |

No umbrella meta-package that references everything.

### 3.2 Dependency graph

```
Mistruna.Core.Abstractions
        ↑
  Mistruna.Core
        ↑
 Mistruna.Core.AspNetCore
        ↑
   EfCore / Caching.Redis / Messaging.RabbitMq /
   Monetization / Observability / Resilience
        ↑
 Mistruna.Core.Testing (refs Core + EfCore as needed)
```

### 3.3 Removed from SDK (not ported)

- `Currency` enum, `CurrencyExtensions`, `CurrencyInfo` (domain; belongs in currency-service)
- `AuthResponse` and other app-specific DTOs currently in Contracts
- Newtonsoft.Json usage and package references
- Legacy sync `RabbitManager` / `IModel` pool design (replaced by new async API)
- BCL duplicate extension methods (`DistinctBy`, `ToHashSet`, `Append`/`Prepend` where BCL already provides them)
- Untested / unused DI `TryAdd*` / `Decorate` helpers (keep only if tests + sample prove need)
- Custom `ErrorResponse` JSON envelope (replaced by ProblemDetails)

### 3.4 Kept (possibly relocated)

- Value objects: `Email`, `PhoneNumber`, `Money`, `Address`, `DateRange` → Abstractions
- Specification + repository interfaces → Abstractions; EF implementations → EfCore
- Monetization feature set → dedicated package (cleaned `AddMistruna*` surface)

---

## 4. Public API & DI surface

### 4.1 Naming

- Prefer `AddMistruna*` / `UseMistruna*` over generic `AddCore` / `UseCoreMiddlewares`
- Namespaces align with packages (e.g. `Mistruna.Core.DependencyInjection`, `Mistruna.Core.Abstractions.Results`)
- Former `Mistruna.Core.Contracts.*` becomes `Mistruna.Core.Abstractions.*`

### 4.2 Canonical composition

```csharp
builder.Services.AddMistrunaCore(opts =>
{
    opts.RegisterAssemblies(typeof(Program).Assembly);
    opts.AddValidation();
    opts.AddLoggingBehavior();
});

builder.Services.AddMistrunaAspNetCore();
builder.Services.AddMistrunaEfCore<AppDbContext>();
builder.Services.AddMistrunaRedis(configuration);
builder.Services.AddMistrunaRabbitMq(configuration);
builder.Services.AddMistrunaObservability(configuration);
builder.Services.AddMistrunaResilience();
// builder.Services.AddMistrunaMonetization(); // optional

var app = builder.Build();
app.UseMistrunaExceptionHandler();
```

No single `UseMistruna()` that enables everything implicitly.

### 4.3 CQRS

- `ICommand`, `ICommand<TResponse>`, `IQuery<TResponse>` live in Abstractions
- `RequestKind` retained for pipeline targeting
- MediatR remains the mediator; Core does not fork MediatR APIs
- FluentValidation behavior maps failures to HTTP 400 ProblemDetails via exception handler

### 4.4 Result vs exceptions

- Expected domain failures → `Result` / `Result<T>`
- HTTP/infrastructure boundary → typed exceptions and/or `Result`→ProblemDetails helpers in AspNetCore
- AspNetCore provides `ToProblemDetails()` / minimal-API result helpers

### 4.5 Serialization & OpenAPI

- System.Text.Json only
- OpenAPI: keep Swashbuckle on STJ for Foundation (re-evaluate OpenAPI.NET in a later wave if needed)
- JWT helpers stay in AspNetCore; API keys / plans stay in Monetization

### 4.6 API quality rules

- Public members require XML docs
- Untested public surface is deleted or made internal
- `InternalsVisibleTo` limited to Testing package + unit test assembly
- Prefer extension-method registration; middleware types not part of casual public surface unless needed for advanced scenarios (`EditorBrowsable` / docs)

---

## 5. Observability

Package: `Mistruna.Core.Observability`

- `AddMistrunaObservability(IConfiguration)` wires OpenTelemetry tracing + metrics using standard ASP.NET / Http instrumentation packages
- Exporters are configuration-driven; do not hard-depend on a single vendor exporter in the library package graph beyond what is required to compile samples
- MediatR `ActivitySource` name: `mistruna.mediator`
- Activity tags: `request.type`, `request.kind` (`command`|`query`), outcome/error
- Exception handler includes `traceId` in ProblemDetails extensions

---

## 6. Resilience

Package: `Mistruna.Core.Resilience`

- Built on `Microsoft.Extensions.Http.Resilience` / Polly v8
- `AddMistrunaResilience()` registers named HttpClient pipeline presets: `Standard`, `Aggressive`, `Disable`
- Optional MediatR behavior retries only requests marked with `[Resilient]` or a marker interface — never global retry on all commands
- All behavior is opt-in and documented

---

## 7. Messaging rewrite (RabbitMQ)

Package: `Mistruna.Core.Messaging.RabbitMq`

- RabbitMQ.Client **7.x**, async-first (`IConnection` / `IChannel`)
- STJ serialization
- Replace legacy `IRabbitManager` sync API with a small explicit bus surface (publish + consume registration)
- Unit-test topology/serialization without requiring a broker in CI; Testcontainers optional follow-up

---

## 8. Samples (Foundation minimum)

1. `samples/Mistruna.Core.Samples.BasicApi` — Core + AspNetCore + CQRS + ProblemDetails
2. `samples/Mistruna.Core.Samples.WorkerRabbit` — publish/consume
3. `samples/Mistruna.Core.Samples.ApiWithOtel` — Observability + Resilience HttpClient

Full documentation site is Wave Brand, not Foundation. Foundation ships root README package table + `docs/migration-v5.md`.

---

## 9. Testing

- Single test project `tests/Mistruna.Core.Tests` with folder-per-package layout
- Required coverage areas:
  - Result/Error
  - Validation + logging behaviors
  - Exception handler → ProblemDetails (status, errorCode, traceId)
  - EfCore repository/UoW (InMemory or SQLite)
  - Redis via fake/`IDistributedCache` (no mandatory Redis in CI)
  - Rabbit serialization/topology unit tests
  - Monetization middleware suite (port existing)
  - Observability Activity tags
  - Resilience HttpClient registration
- Testing package smoke-tested
- Keep `TreatWarningsAsErrors`; reduce blanket nullability `NoWarn` on public API
- Recommended: `PublicApiAnalyzers` baselines per packable project

---

## 10. CI / Release / Versioning

- CI: restore → `dotnet format --verify-no-changes` → build → test (coverage upload) → pack all packable projects
- Fail if packable projects lack required package metadata (README/license/icon as applicable)
- Release on tag `v5.0.0` (MinVer prefix `v`) pushes all nupkgs + snupkgs
- All modules share the same release-train version
- v4.x line: frozen on NuGet; README states migrate to v5 packages
- Dependabot retained; group Microsoft.* / OpenTelemetry.* where practical

---

## 11. Migration path

Mandatory doc: `docs/migration-v5.md` covering:

1. Old → new package reference map
2. Namespace / API rename table (`AddCore` → `AddMistrunaCore`, etc.)
3. Newtonsoft → STJ
4. Currency removal guidance
5. Rabbit old manager → new async bus
6. `ErrorResponse` → ProblemDetails
7. Checklist for `user-service` / `currency-service` (executed as follow-up outside this Core Foundation merge)

Consumer migrations are **not** part of the Foundation implementation merge in Mistruna.Core.

---

## 12. Success criteria

Foundation is done when:

- [ ] Solution builds on .NET 10; all tests green
- [ ] All section 3.1 packages exist, pack, and appear in README table
- [ ] Currency / Newtonsoft / legacy Rabbit manager are gone
- [ ] BasicApi + WorkerRabbit + ApiWithOtel samples run
- [ ] `docs/migration-v5.md` complete
- [ ] Repository is tag-ready for `v5.0.0`

---

## 13. Follow-up waves (not this spec)

1. **Brand & Docs** — identity, docs site, contributor UX polish
2. **Platform+** — source generators, analyzers, deeper conventions
3. **Ecosystem** — migrate GitLab services to v5, community/release communication
