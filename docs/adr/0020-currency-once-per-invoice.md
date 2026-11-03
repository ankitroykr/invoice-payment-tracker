---
status: accepted
date: 2026-11-03
decision-makers: Ankit Kumar Roy
---

# Store the currency once per invoice

## Context and Problem Statement

ADR 0009 stores every `Money` as an amount and a currency. On invoice lines that saves the same currency again for every amount, even though every line has to be in the invoice's currency anyway.

Where should an invoice line's currency be stored?

## Considered Options

* Once, on the invoice
* With every amount on the line

## Decision Outcome

Chosen option: "Once, on the invoice", because a line can't have its own currency, and invoicing and accounting systems usually keep it on the invoice.

* Line unit prices and totals are plain `decimal(19,4)` columns.
* `Invoice.Total` is still `Money`, built from the lines and the invoice's currency.
* `AddLine` still takes `Money`, so a unit price in the wrong currency is still rejected.
* ADR 0009 still applies to `Money` everywhere else.

### Consequences

* Good, because each currency is stored once and can't disagree with itself.
* Bad, because line amounts are plain decimals, so the type no longer carries the currency.

## Pros and Cons of the Options

### With every amount on the line

* Good, because each amount maps straight to `Money`.
* Bad, because it saves the same currency again for every amount on every line.
