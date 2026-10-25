---
status: accepted
date: 2026-10-25
decision-makers: Ankit Kumar Roy
---

# Return a Result for expected business failures

## Context and Problem Statement

Some actions get refused for business reasons, like issuing an invoice that's already issued. The domain has to say no, and the API has to turn that into the right response.

Should the domain throw exceptions or return results?

## Considered Options

* Throw domain exceptions and map them in one handler
* Return my own `Result` and `Error` types
* Use a library such as Ardalis.Result or ErrorOr

## Decision Outcome

Chosen option: "My own `Result` and `Error` types", because a refusal is a normal outcome, and I want to learn the pattern by building it.

* `Error` has a stable `Code` (e.g. `Invoice.NotDraft`) and a `Message` for people.
* `Result` is a success, or a failure with an `Error`. `Result<TValue>` also carries a value on success.
* Results are only created through `Result.Success(...)` and `Result.Failure(...)`.
* Exceptions are only for bugs, like a null argument. `UseExceptionHandler()` still turns those into a 500.
* One helper in the API will map an `Error` to Problem Details.

### Consequences

* Good, because the method signature shows an action can fail.
* Good, because the Domain stays free of packages.
* Bad, because callers must check the result. An ignored failure is silently lost.

## Pros and Cons of the Options

### Domain exceptions

* Good, because it's less code, and Microsoft's design guidelines favour exceptions.
* Bad, because a refusal doesn't show in the signature.

### My own `Result` and `Error`

* Good, because it's small and I understand every line.
* Bad, because I maintain it myself.

### Ardalis.Result or ErrorOr

* Good, because they're tested and full-featured.
* Neutral, because I may switch later, with its own ADR.

## More Information

* https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/exception-throwing
