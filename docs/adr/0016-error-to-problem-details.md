---
status: accepted
date: 2026-10-26
decision-makers: Ankit Kumar Roy
---

# Map errors to Problem Details by error type

## Context and Problem Statement

Handlers return a `Result` (ADR 0010). `Error` only has a `Code` and a `Message`, so the API can't tell "customer not found" from "email already used".

How does the API pick the status code?

## Considered Options

* An `ErrorType` on `Error`
* Work it out from the code, e.g. a code ending in `NotFound` means 404
* A lookup in the API from each code to a status

## Decision Outcome

Chosen option: "An `ErrorType` on `Error`", because each kind of error is mapped once. ErrorOr and Ardalis.Result do the same.

| `ErrorType` | Status | Example |
|---|---|---|
| `Validation` | 400 | Name is missing |
| `NotFound` | 404 | Customer 42 doesn't exist |
| `Conflict` | 409 | Email already used |
| `BusinessRule` | 422 | Issuing an invoice that's already issued |
| Not mapped | 500 | A bug |

* Errors are created through factory methods, e.g. `Error.NotFound(code, message)`.
* One helper in the API does the mapping, so every response has the same Problem Details shape.

### Consequences

* Good, because a new error never needs an API change.
* Neutral, because 409 vs 422 is debated. Some teams use one for everything.

## Pros and Cons of the Options

### Work it out from the code

* Bad, because a typo silently becomes a 500.

### Lookup per code

* Bad, because every new code needs an entry.

## More Information

* https://www.rfc-editor.org/rfc/rfc9110.html#name-409-conflict
* https://www.rfc-editor.org/rfc/rfc9110.html#name-422-unprocessable-content
* https://github.com/ardalis/Result/blob/main/src/Ardalis.Result.AspNetCore/ResultStatusMap.cs
