---
status: accepted
date: 2026-11-12
decision-makers: Ankit Kumar Roy
---

# Take invoice numbers from a counter per year

## Context and Problem Statement

ADR 0017 gives an invoice its number when it's issued, with a counter that restarts each year. It left open how the next number is worked out when two invoices are issued at the same time.

UK rules ask for a unique number. A rare gap, for example when an issue fails after taking its number, is acceptable. This relaxes the "no gaps" wording in ADR 0017; the rest of ADR 0017 still applies.

## Considered Options

* A counter row per year, increased in one atomic update
* Highest number in the year plus 1
* A SQL Server sequence

## Decision Outcome

Chosen option: "A counter row per year, increased in one atomic update", because concurrent issues each get the next number straight away, with no retries.

* One statement increases the year's counter and returns the new value. The row lock lasts only for that statement.
* The year's row is added the first time an invoice is issued that year.
* The unique index on year and counter stays as a last check.

### Consequences

* Good, because concurrent issues never get the same number and don't fail.
* Good, because the counter restarts each year with no scheduled job.
* Bad, because a number is lost if the issue fails after taking it.
* Bad, because it adds a table and a raw SQL statement.

## Pros and Cons of the Options

### Highest number in the year plus 1

* Good, because it needs no extra table.
* Bad, because two issues at the same moment read the same number, and one of them fails.

### A SQL Server sequence

* Good, because SQL Server hands out numbers safely, without locks.
* Bad, because it doesn't restart each year, so it would need a sequence per year or a yearly reset.

## More Information

* https://www.gov.uk/invoicing-and-taking-payment-from-customers/invoices-what-they-must-include
* https://learn.microsoft.com/en-us/sql/relational-databases/sequence-numbers/sequence-numbers
* https://learn.microsoft.com/en-us/sql/t-sql/queries/output-clause-transact-sql
