---
status: accepted
date: 2026-10-25
decision-makers: Ankit Kumar Roy
---

# Represent money as a value object with amount and currency

## Context and Problem Statement

A plain `decimal` can't tell an amount from a quantity or a VAT rate, and it has no currency. I've seen bugs from exactly that in payments work.

How should the code hold an amount, and how should SQL Server store it?

## Considered Options

* A plain `decimal`
* A `Money` value object with amount and currency
* A third-party money library

## Decision Outcome

Chosen option: "A `Money` value object", because these mistakes then fail straight away instead of reaching an invoice.

* `Money` is an immutable `sealed record` with `Amount` and `Currency` (ISO 4217, e.g. GBP). It's a class, not a struct, so `default` can't skip the validation.
* `+` and `-` throw if the currencies differ, and a bad currency code throws. Those are bugs, so they're exceptions.
* Amounts can be negative. "A line can't be negative" is an `Invoice` rule.
* Stored as `decimal(19,4)` and `char(3)`. `decimal(19,4)` takes the same 9 bytes as `decimal(18,2)`, so the extra precision is free.
* GBP only for now. Rounding gets decided with invoice totals.

### Consequences

* Good, because mixing currencies fails early.
* Good, because sub-penny values survive until the final rounding.
* Bad, because every amount needs mapping in EF Core.

## Pros and Cons of the Options

### A plain `decimal`

* Good, because it's simple.
* Bad, because nothing stops the mistakes above.

### A `Money` value object

* Good, because the rules live in one small type I own.

### A third-party money library

* Bad, because it adds a package to the Domain for something small.

## More Information

* https://learn.microsoft.com/en-us/sql/t-sql/data-types/decimal-and-numeric-transact-sql
