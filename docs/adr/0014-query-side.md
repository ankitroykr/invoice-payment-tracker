---
status: accepted
date: 2026-10-26
decision-makers: Ankit Kumar Roy
---

# Read data through query interfaces, using EF Core first

## Context and Problem Statement

Get customer by ID is the first read, and a customer list with paging comes next. Writes go through the domain and repositories, but reads have no rules to check. They just need data in the shape of the response. Application can't reference EF Core (ADR 0007).

How should reads get their data?

## Considered Options

* Reuse the repository and map the entity
* A query interface in Application, implemented with EF Core in Infrastructure
* The same interface, implemented with Dapper

## Decision Outcome

Chosen option: "A query interface implemented with EF Core", because reads skip the domain, and Application stays free of EF Core.

* `ICustomerQueries` returns DTOs like `CustomerResponse`.
* `CustomerQueries` uses `AsNoTracking()` and `Select(...)`, so it only reads the columns it needs.
* If a read gets heavy, I can rewrite that one query with Dapper and change its DI registration. Integration tests against real SQL Server will catch wrong column names.

### Consequences

* Good, because the same pattern works for lists.
* Bad, because there are two ways into the database: repositories for writes, queries for reads.

## Pros and Cons of the Options

### Reuse the repository

* Bad, because it loads the whole entity and doesn't suit lists with paging.

### Dapper from the start

* Bad, because it's a second data access style before I need one.

## More Information

* https://learn.microsoft.com/en-us/ef/core/querying/tracking
* https://github.com/dotnet/eShop/blob/main/src/Ordering.API/Application/Queries/IOrderQueries.cs
