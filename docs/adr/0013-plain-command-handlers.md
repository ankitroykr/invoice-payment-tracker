---
status: accepted
date: 2026-10-26
decision-makers: Ankit Kumar Roy
---

# Call use cases through plain command handlers

## Context and Problem Statement

Each use case is a command (the input) and a handler (the code that runs it), starting with create customer.

What should the controller call to run it?

## Considered Options

* MediatR
* Mediator (martinothamar/Mediator)
* A plain handler class, injected into the controller

## Decision Outcome

Chosen option: "A plain handler class", because I can follow the call straight from the controller into the handler, with no library in between.

* `CreateCustomerHandler.HandleAsync` runs the validator, calls the domain and returns a `Result`.
* Handlers are registered in DI and injected into the controller.

### Consequences

* Good, because there's no package or licence key.
* Bad, because each handler calls its validator itself, instead of one MediatR pipeline behaviour doing it for all.
* Neutral, because MediatR handlers have the same shape, so switching later is small.

## Pros and Cons of the Options

### MediatR

* Good, because it's common in enterprise .NET, and Microsoft's eShop uses it.
* Bad, because it's been commercial since v13. I'd qualify for the free Community licence, but still need a key.

### Mediator

* Good, because it's MIT licensed.
* Bad, because few teams use it.

## More Information

* https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/microservice-application-layer-implementation-web-api
* https://mediatr.io/#pricing
