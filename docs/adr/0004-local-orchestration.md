---
status: accepted
date: 2026-10-21
decision-makers: Ankit Kumar Roy
---

# Run SQL Server locally with Docker Compose, moving to .NET Aspire later

## Context and Problem Statement

Locally, the API needs SQL Server. For now that's the only dependency, but later the app adds a worker, Service Bus and an Angular app, and starting all of that by hand won't scale.

How should local dependencies be run?

## Decision Drivers

* Quick, repeatable setup, not a manual install.
* The same database engine as production.
* Data survives restarts, and secrets stay out of the repo.
* The setup shouldn't be bigger than the problem.

## Considered Options

* Install SQL Server on the machine
* Docker Compose for SQL Server, API run from the IDE
* .NET Aspire from the start
* A shared cloud database for development

## Decision Outcome

Chosen option: "Docker Compose for SQL Server, API run from the IDE", because one small file covers the one dependency we have. I'll move to .NET Aspire when there's more to run, and supersede this record.

* `compose.yaml` runs SQL Server 2022 on port 1433, with a named volume and a health check.
* The `sa` password lives in a git-ignored `.env` file; `.env.example` shows what to set.
* The API runs from Rider or `dotnet run`, with its connection string in user secrets.

### Consequences

* Good, because `docker compose up -d` is all it takes, and it's real SQL Server.
* Bad, because the API and database are started separately, and the connection string is set up by hand.
* Bad, because the compose file uses the `2022-latest` tag, so the version can change under me. The tests pin a CU instead.

### Confirmation

The README setup works from a clean clone, and `/health/ready` returns `Healthy`.

## Pros and Cons of the Options

### Install SQL Server on the machine

* Good, because there's no container layer.
* Bad, because it's a heavy install that's hard to reset, and it doesn't run natively on every OS.

### Docker Compose, API from the IDE

* Good, because it's small, standard and easy to reset.
* Bad, because every extra service means more hand-written config.

### .NET Aspire from the start

* Good, because one command starts everything, with a local telemetry dashboard.
* Bad, because it's two extra projects for a single database.

### Shared cloud database

* Good, because there's nothing to run locally.
* Bad, because it costs money and shared data makes work unpredictable.

## More Information

* Introduced in PR #5.
* .NET Aspire overview: https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview
