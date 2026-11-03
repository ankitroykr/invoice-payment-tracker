---
status: accepted
date: 2026-10-30
decision-makers: Ankit Kumar Roy
---

# Number invoices per year, assigned on issue

## Context and Problem Statement

Every invoice needs a unique number, and invoice numbers are expected to run in sequence with no gaps. Drafts can be deleted, so a number given to a draft can be lost.

When does an invoice get its number, and what does it look like?

## Considered Options

* On issue, `INV-<year>-<counter>`, with the counter restarting each year
* On issue, one running number, e.g. `INV-0001`
* On create, when the draft is saved

## Decision Outcome

Chosen option: "On issue, `INV-<year>-<counter>`, with the counter restarting each year", because issued invoices get numbers with no gaps, and the year shows at a glance.

* A draft has no number. Issuing it gives it the next one.
* The counter has at least 4 digits: `INV-2026-0001`. That's a minimum, not a limit, so the 10,000th invoice in a year is `INV-2026-10000`.
* The year and the counter are also stored as numbers, and lists sort on those. Sorting the text would put `INV-2026-10000` before `INV-2026-2000`.
* There's no sign-in yet, so numbers are unique across all invoices. Once invoices have an owner, they're unique per owner.

### Consequences

* Good, because deleting a draft never leaves a gap.
* Good, because anyone can tell the year from the number.
* Bad, because there's one counter per year instead of one overall.
* Neutral, because how the next number is generated safely is decided with invoice persistence.

## Pros and Cons of the Options

### One running number

* Good, because it's the simplest to generate.
* Bad, because the number doesn't say when the invoice was issued.

### On create

* Bad, because deleting a draft leaves a gap.

## More Information

* https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#decimal-format-specifier-d
