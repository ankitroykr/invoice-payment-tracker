---
status: accepted
date: 2026-10-25
decision-makers: Ankit Kumar Roy
---

# Use a generic repository per aggregate root with a unit of work

## Context and Problem Statement

Use cases need to load and save aggregates like `Customer` without knowing about EF Core, so the Domain and Application layers stay free of it (ADR 0007).

How should the rest of the app reach the database?

## Considered Options

* Use `DbContext` directly
* A generic `IRepository<T>` per aggregate root, with the unit of work on the repository
* A unit of work that holds every repository (`uow.Customers`, `uow.CompleteAsync()`)

## Decision Outcome

Chosen option: "A generic `IRepository<T>` per aggregate root", because it keeps EF Core out of the inner layers and follows Microsoft's eShop guidance.

* `IAggregateRoot` is an empty marker. `IRepository<T> where T : class, IAggregateRoot` only accepts aggregate roots, and the compiler enforces it.
* Each aggregate gets its own interface, e.g. `ICustomerRepository : IRepository<Customer>`, for its own queries later.
* `IUnitOfWork` has one method, `SaveChangesAsync`. `InvoiceTrackerDbContext` implements it with no extra code.
* Repositories expose `UnitOfWork`. They don't save.
* An abstract `Repository<T>` in Infrastructure holds the shared EF Core code.

### Consequences

* Good, because the inner layers don't reference EF Core and can be tested with fakes.
* Good, because a new aggregate adds a repository without changing existing code.
* Bad, because `DbContext` already is a repository and unit of work, so these are thin wrappers.

## Pros and Cons of the Options

### `DbContext` directly

* Good, because it's the least code, and Microsoft documents it as a valid choice.
* Bad, because the Application layer would depend on EF Core.

### Generic repository per aggregate root

* Good, because the shared code lives in one place.
* Neutral, because it stays small on purpose. A full CRUD generic repository over EF Core is widely seen as an anti-pattern.

### Unit of work holding every repository

* Good, because a use case injects one thing.
* Bad, because `IUnitOfWork` changes for every new aggregate and gives every use case every repository.

## More Information

* Transactions across several saves (`BeginTransactionAsync`) will be added when payments need them.
* https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design
