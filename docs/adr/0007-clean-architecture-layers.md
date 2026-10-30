---
status: accepted
date: 2026-10-25
decision-makers: Ankit Kumar Roy
---

# Organise the solution in Clean Architecture layers

## Context and Problem Statement

Customers and invoices bring the first business rules. Until now everything sat in the API project, EF Core included. I want the rules separate from the database and the web, and quick to unit test.

How should I split the code into projects?

## Considered Options

* A `Domain` folder inside the API
* A separate Domain project, EF Core stays in the API
* Full Clean Architecture: Domain, Application, Infrastructure, API

## Decision Outcome

Chosen option: "Full Clean Architecture", because it's what I see in most enterprise .NET code and each project has one job.

* Domain references nothing. Application references Domain, Infrastructure references Application, and the API references both.
* I add each project when it has real code: Domain now, Infrastructure when EF Core moves out of the API, Application with the first use case.
* Argument checks use the built-in .NET throw helpers. I tried Ardalis.GuardClauses, but the built-ins cover what I need with no package, and Microsoft's analysers recommend them.

### Consequences

* Good, because the Domain project has no EF Core or ASP.NET Core reference, so its code can't use them by mistake. The build would fail.
* Good, because domain tests need no Docker and run in under a second.
* Bad, because there are more projects to look after.

## Pros and Cons of the Options

### A `Domain` folder inside the API

* Good, because there's nothing to set up.
* Bad, because nothing stops domain code using EF Core.

### A separate Domain project

* Good, because it gets most of the benefit with fewer projects.
* Bad, because the API ends up holding web, use case and database code.

### Full Clean Architecture

* Good, because it's widely known and each layer is clear.
* Bad, because it's more ceremony for a small app.

## More Information

* Builds on ADR 0003, which planned a separate domain project.
* https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures
