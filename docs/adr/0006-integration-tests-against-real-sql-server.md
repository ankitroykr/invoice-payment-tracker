---
status: accepted
date: 2026-10-21
decision-makers: Ankit Kumar Roy
---

# Run integration tests against a real SQL Server with Testcontainers

## Context and Problem Statement

The integration tests run the API in memory with `WebApplicationFactory` and call it over HTTP. The API uses SQL Server through EF Core, so the tests need a database, and a fake one can pass while the real one fails.

What database should the tests run against?

## Decision Drivers

* Tests should catch what production would, including SQL Server behaviour and migrations.
* Every run should start clean.
* No setup beyond Docker, locally or in CI.

## Considered Options

* EF Core in-memory provider
* SQLite in-memory
* Real SQL Server via Testcontainers
* A shared test database

## Decision Outcome

Chosen option: "Real SQL Server via Testcontainers", because it's the same engine as production and every run gets a fresh, throwaway database.

* `InvoiceTrackerApiFactory` starts `mssql/server:2022-CU27-ubuntu-22.04`, pinned so test runs don't change when a new CU comes out.
* The factory is an assembly fixture, so all test classes share one container and one API host (see ADR 0005).
* Testcontainers removes the container when the run finishes.

### Consequences

* Good, because tests exercise real SQL Server, and nothing is left behind.
* Bad, because Docker has to be running, and the first run is slow while the image downloads.
* Bad, because all tests share one database. That's fine while tests only read, but tests that write will need their own data or a reset between tests.

### Confirmation

The tests pass locally and in CI, where Docker is available on the GitHub runner.

## Pros and Cons of the Options

### EF Core in-memory provider

* Good, because it's fast.
* Bad, because it isn't relational, and Microsoft's EF Core testing guidance strongly discourages it as a database fake.

### SQLite in-memory

* Good, because it's fast and relational.
* Bad, because it's a different engine, so SQL Server behaviour goes untested.

### Real SQL Server via Testcontainers

* Good, because it's the production engine, clean every run.
* Bad, because it's slower to start and needs Docker.

### Shared test database

* Good, because there's nothing to start.
* Bad, because shared data makes runs unrepeatable, and CI would need access to it.

## More Information

* Introduced in PR #6; shared fixture added in PR #12.
* Microsoft, "Choosing a testing strategy" (EF Core): https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy
* Testcontainers SQL Server module: https://dotnet.testcontainers.org/modules/mssql/
