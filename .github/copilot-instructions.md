# AGENTS.md – PrinterRepair

If you're helping out with this repository, please read through these guidelines before writing or changing any code. This project follows Clean Architecture, DDD, and CQRS, and we use automated tests to keep those boundaries intact.

## 1. Where are the Design Docs?
Don't guess class names, exceptions, or business rules. Everything is documented in `docs/`:
- `docs/architecture/aggregates-and-invariants.md` – Our Ubiquitous Language, Value Objects (`Money`, `PartQuantity`, `TaxId`, `PrinterModel`), strongly-typed IDs (`ClientAccountId`, `RegisteredPrinterId`, `RepairTicketId`, `SparePartsInventoryId`, `ServiceInvoiceId`), business invariants, and Given-When-Then scenarios.
- `docs/architecture/stateDiagram-*.md` – The lifecycle of `RepairTicket` and which operations are blocked in each state.
- `docs/architecture/context-map.md` – How `Identification`, `Diagnostics`, and `BAH` fit together, plus the exact port contracts and mapping tables for our Anti-Corruption Layer (ACL).
- `docs/architecture/fitness-functions.md` – Automated architectural fitness functions (`FF-01`–`FF-07`), cross-service isolation rules, and contract versioning / Tolerant Reader policies.
- `docs/contracts/openapi-bah.yaml` – OpenAPI 3.1 HTTP contract for the `BAH` microservice (`/api/v1/invoices/*`), including required `Idempotency-Key` headers and RFC 9457 `ProblemDetails` responses.

Stick to the exact names defined in those files.

If the existing implementation conflicts with the architecture documentation, follow the documented design rather than copying the existing implementation. If the documentation itself is ambiguous or incomplete, explain the issue before introducing a new architectural decision.

## 2. Commands to Run Before Finishing
Whenever you make a change, verify that everything builds and passes:
- Build: `dotnet build --no-restore`
- Run all tests: `dotnet test --no-build`
- Run architecture guards only: `dotnet test --filter "FullyQualifiedName~CleanArchitectureGuardTests"`

Nothing is considered done until `dotnet test` comes back with Failed: 0.

## 3. How We Work (Plan -> Red -> Green)
Please avoid generating whole features in one massive dump. Take it step by step:
1. **Plan & Contracts:** Start by outlining the files and writing the port interfaces (`.Application/Ports`), CQRS contracts, and domain exceptions (`.Domain/Exceptions`).
2. **Red (TDD):** Write the unit tests first based on the Given-When-Then scenarios or ACL mapping tables, and confirm they compile and fail for the expected reason (e.g. NotImplementedException or a failed assertion, not a build error).
3. **Green:** Write the simplest, cleanest code needed to make the tests pass.
4. **Verify:** Run `CleanArchitectureGuardTests` (`NetArchTest.Rules`) to confirm no layer rules were accidentally broken.

## 4. Project & Layer Structure
Each Bounded Context (`Diagnostics`, `BAH`, `Identification`) is split into 4 projects:
- `PrinterRepair.<Context>.Domain` – Only business logic. Keep it completely free of external NuGet packages (no EF Core, no JSON libraries) and references to other layers.
- `PrinterRepair.<Context>.Application` – CQRS Commands/Queries (`Features/`), messaging interfaces (`Abstractions/Messaging/`), repository interfaces (`Repositories/I*Repository`), and outbound ports (`Ports/I*Port`). Only references `.Domain`.
- PrinterRepair.<Context>.Infrastructure – Database and external integration details (DbContext, IEntityTypeConfiguration<T> mappings, repositories, and Acl/). Aside from DependencyInjection, keep all implementation classes here internal sealed.
- `PrinterRepair.<Context>.Presentation` – ASP.NET Core controllers that hand off work to CQRS handlers, request contracts, and `Program.cs`.

## 5. Domain Model Conventions (`.Domain`)
- **No public setters:** Use `private set` or get-only `{ get; }` properties on all entities, aggregates, and Value Objects.
- **Protected collections:** Keep lists private (`private readonly List<T> _items = new();`) and expose them as `IReadOnlyCollection<T>`.
- **EF Core support:** Include a `private` parameterless constructor on entities and aggregates so EF Core can materialize them, but don't put [Table], [Key], [Column], or [ForeignKey] attributes in .Domain.
- **Exceptions:** When a business rule is broken, throw a `sealed` exception inheriting from `DomainException` with a unique `snake_case` error code.

## 6. CQRS, EF Core, and ACL Rules
- **Separate reads and writes:** Use `ICommand<TResponse>` / `ICommandHandler<TCommand, TResponse>` for anything that changes state, and `IQuery<TResponse>` / `IQueryHandler<TQuery, TResponse>` for read-only lookups.
- **Change tracking:** Keep EF Core tracking enabled when loading an aggregate to modify it (`GetByIdAsync`), and include child collections as needed. Only use `.AsNoTracking()` in read-only queries (`GetByIdReadOnlyAsync`, `GetAllAsync`).
- **Keep external models out:** Domain ports (`I*Port`) live in `.Application/Ports/` and should only use our own domain types. Any external system DTOs (`*_DTO`) belong in `.Infrastructure/Acl/ExternalContracts/` as `internal sealed` classes.
- **ACL structure:** Split each integration into three `internal sealed` classes: `*Translator` (pure, side-effect-free data/error mapping using CultureInfo.InvariantCulture, zero network I/O), `*Adapter` (handles the actual I/O), and `*Acl` (the facade implementing `I*Port`).

## 7. Unit Testing (xUnit)
- Stick to the `Arrange – Act – Assert` pattern and name tests clearly: `Method_WhenCondition_ShouldExpectedResult`.
- Test real domain objects and ACL translators directly without mocking libraries like `Moq`. Avoid writing tests that just verify a mock returned what you told it to return.

## 8. Architectural Fitness Function Rules (`FF-01`–`FF-07`)
- **FF-01:** Never add EF Core, ASP.NET Core, `System.Net.Http`, or message broker references to any `*.Domain` project.
- **FF-02:** Never add `public set` to any property in `*.Domain`; mutate state exclusively through domain methods and keep all `DomainException` subclasses `sealed`.
- **FF-03:** Never reference `*.Infrastructure` or `*.Presentation` from `*.Application`; declare all `*CommandHandler` and `*QueryHandler` classes as `internal sealed`.
- **FF-04:** Keep all external DTOs (`*_DTO`), translators, and adapters `internal sealed` inside `*.Infrastructure.Acl`; never expose them through ports in `*.Application`.
- **FF-05:** Never add `<ProjectReference>` entries, namespace imports, cross-schema `FOREIGN KEY`s, or SQL `JOIN`s between `PrinterRepair.Diagnostics.*`, `PrinterRepair.BAH.*`, and `PrinterRepair.Identification.*`.
- **FF-06 / FF-07:** Keep `PrinterRepair.BAH` controllers and `PrinterRepair.Diagnostics` HTTP adapters aligned with `docs/contracts/openapi-bah.yaml`, enforce `Idempotency-Key` on all `POST` endpoints, and run `dotnet test` after every edit.