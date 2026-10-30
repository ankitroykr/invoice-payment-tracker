---
status: accepted
date: 2026-10-26
decision-makers: Ankit Kumar Roy
---

# Validate requests with FluentValidation in the Application layer

## Context and Problem Statement

Customers now come in through the API, and later payments will come through minimal APIs and reminders through Service Bus. Bad input should get a 400 with `ValidationProblemDetails`. The domain guards are there to catch bugs, not user mistakes.

Where should input validation live?

## Considered Options

* DataAnnotations on the API request DTOs
* FluentValidation with its automatic ASP.NET Core validation
* FluentValidation in the Application layer

## Decision Outcome

Chosen option: "FluentValidation in the Application layer", because the rules apply however a request arrives, not just over HTTP.

* One validator per command, e.g. `CreateCustomerCommandValidator`, run before the domain is called.
* The API turns a failure into a 400 `ValidationProblemDetails`.
* The domain keeps its own guards.

### Consequences

* Good, because validators are plain classes I can unit test.
* Bad, because Application gets a package, and a few rules exist in both the validator and the domain.

## Pros and Cons of the Options

### DataAnnotations

* Good, because it's built in and `[ApiController]` returns the 400 for me.
* Bad, because it only runs on HTTP requests, and async rules are awkward.

### Automatic FluentValidation

* Bad, because FluentValidation no longer recommends it for new projects. It can't run async rules and only works with MVC.

### FluentValidation in Application

* Good, because it works for controllers, minimal APIs and message handlers.

## More Information

* https://docs.fluentvalidation.net/en/latest/aspnet.html
* https://github.com/dotnet/eShop/blob/main/src/Ordering.API/Application/Validations/CreateOrderCommandValidator.cs
