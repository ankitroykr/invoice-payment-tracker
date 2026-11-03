---
status: accepted
date: 2026-11-03
decision-makers: Ankit Kumar Roy
---

# Support a fixed list of currencies and round to their minor units

## Context and Problem Statement

A line of 4 × 55 came back as `220.00000000`, because `decimal` keeps its scale. Currencies don't all have two decimal places: ISO 4217 gives GBP 2, JPY 0 and KWD 3.

Which currencies can an invoice use, and how are amounts rounded?

## Considered Options

* A currencies table with each currency's ISO 4217 minor units
* Two decimal places for every currency
* Decimal places from `CultureInfo`

## Decision Outcome

Chosen option: "A currencies table", because each currency rounds to its own decimal places, and a new currency is a new row, not new code.

* GBP, EUR, USD and INR for now, all with 2 minor units.
* The migration seeds the table, and invoices have a foreign key to it.
* Line totals are rounded half away from zero (0.125 to 0.13), not to even (0.125 to 0.12). The invoice total is the sum of the rounded lines.
* Unit prices keep up to 4 decimal places. Only totals are rounded.
* Line totals and the currency's minor units are stored on the invoice, so saved invoices never change.

### Consequences

* Good, because the lines always add up to the total.
* Good, because a currency with 0 or 3 decimal places is just a new row.
* Bad, because the currency code is checked in more than one place: its format in the validator and the domain, and whether it's supported in the database.

## Pros and Cons of the Options

### Two decimal places for every currency

* Bad, because it's wrong for JPY and KWD.

### Decimal places from `CultureInfo`

* Bad, because it describes a country's formatting, not the currency, and the data comes from the operating system.

## More Information

* https://www.iso.org/iso-4217-currency-codes.html
* https://learn.microsoft.com/en-us/dotnet/api/system.midpointrounding
