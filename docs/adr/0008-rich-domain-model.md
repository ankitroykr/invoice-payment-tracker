---
status: accepted
date: 2026-10-25
decision-makers: Ankit Kumar Roy
---

# Use a rich domain model where there are business rules

## Context and Problem Statement

Invoices have real rules. Status goes Draft, Issued, then Paid or Void, and an issued invoice can't be edited. Payments will add more.

Should the rules live in the entities or in service classes?

## Considered Options

* Anaemic model: public setters, rules in services
* Rich model: entities protect their own rules
* Full DDD toolkit: rich model plus domain events and specifications

## Decision Outcome

Chosen option: "Rich model", because then a rule can't be skipped by accident.

* Properties have private setters, and state only changes through methods like `Issue()` that check the rules.
* Collections are read-only from outside.
* Entities without rules, like `Customer`, stay simple.
* I'll add domain events or other DDD patterns only when I actually need them.

### Consequences

* Good, because an invalid invoice can't be built from outside.
* Good, because tests are simple: create, call a method, check.
* Bad, because EF Core needs a bit more mapping.

## Pros and Cons of the Options

### Anaemic model

* Good, because it's quick and familiar.
* Bad, because any code can set `Status` and skip the rules.

### Rich model

* Good, because the rules sit next to the data they protect.

### Full DDD toolkit

* Bad, because it's too much for the rules this app has today.

## More Information

* https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/microservice-domain-model
