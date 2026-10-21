---
status: accepted
date: 2026-10-21
decision-makers: Ankit Kumar Roy
---

# Use controllers and minimal APIs, split by area

## Context and Problem Statement

ASP.NET Core supports controller-based APIs and Minimal APIs. Microsoft recommends Minimal APIs for new projects. But nearly every enterprise codebase I've worked on uses controllers, and this project is partly a portfolio, so I want to show both.

Which style should the API use?

## Decision Drivers

* Each part of the API should be predictable to read.
* I want to keep practising controllers and learn Minimal APIs properly.
* Shared behaviour (errors, validation, OpenAPI) must work the same in both.

## Considered Options

* Controllers only
* Minimal APIs only
* Both, split by area
* FastEndpoints

## Decision Outcome

Chosen option: "Both, split by area", because I get to use both styles without mixing them randomly.

* Customers and invoices: controllers.
* Payments and the dashboard: Minimal APIs with endpoint groups.
* Health checks: the built-in `MapHealthChecks`.

### Consequences

* Good, because each area is consistent and I get experience with both.
* Bad, because there are two patterns to learn, and validation and auth need testing in both. Problem Details already covers both, as it's middleware.

If mixing styles turns out not to be worth it, I'll move to Minimal APIs and supersede this record.

### Confirmation

In PR reviews, new endpoints follow their area's style.

## Pros and Cons of the Options

### Controllers only

* Good, because it's what most .NET teams know.
* Bad, because it's no longer Microsoft's recommendation for new projects.

### Minimal APIs only

* Good, because it's the recommended approach, with less ceremony.
* Bad, because I wouldn't practise the style I'll meet in most existing codebases.

### Both, split by area

* Good, because both styles are used deliberately, each in its own area.
* Bad, because cross-cutting behaviour has to work in both.

### FastEndpoints

* Good, because it gives Minimal APIs a class-per-endpoint structure.
* Bad, because it's an extra dependency and rare in enterprise code.

## More Information

* Microsoft, "APIs overview": https://learn.microsoft.com/en-us/aspnet/core/fundamentals/apis
