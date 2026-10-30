---
status: accepted
date: 2026-10-21
decision-makers: Ankit Kumar Roy
---

# Record architecture decisions as MADR files in the repository

## Context and Problem Statement

By the time the first version of the API was running, I'd made several decisions that shape this project. The code shows what I chose, but not why, or what I turned down. Some of it is in PR descriptions, but I won't find it there in six months.

Where should these decisions be written down?

## Decision Drivers

* The reasoning, including rejected options, should outlive the PR.
* Records should live in the repo and be reviewed like code.
* Keep it light: one short file per decision.

## Considered Options

* No formal records
* Michael Nygard's ADR format
* MADR 4.0
* An external wiki (Confluence or the GitHub wiki)

## Decision Outcome

Chosen option: "MADR 4.0", because it's plain Markdown in the repo and has a section for the options considered, which is the bit that usually gets lost.

* Records go in `docs/adr/` as `NNNN-short-title.md`.
* I've dropped the optional `consulted` and `informed` fields while it's just me.
* Accepted records aren't edited. If I change my mind, a new record supersedes the old one.
* Only decisions that shape the system or are hard to undo get a record.

### Consequences

* Good, because the why sits next to the code and gets reviewed with it.
* Bad, because it's extra writing, and I've had to write up my first decisions after the fact.

### Confirmation

PR reviews check that significant changes come with a record.

## Pros and Cons of the Options

### No formal records

* Good, because there's nothing to maintain.
* Bad, because the reasoning gets scattered and the alternatives are rarely written down.

### Michael Nygard's ADR format

* Good, because it's short and well known.
* Bad, because there's no section for rejected options, so they tend to get skipped.

### MADR 4.0

* Good, because it has sections for options and their pros and cons, and most sections are optional.
* Neutral, because it's a bit longer than Nygard's.

### An external wiki

* Good, because it's familiar from work.
* Bad, because it sits outside the repo and isn't versioned with the code.

## More Information

* Michael Nygard, "Documenting Architecture Decisions": https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions
* MADR: https://adr.github.io/madr/
* Microsoft, "Architecture decision record": https://learn.microsoft.com/en-us/azure/well-architected/architect-role/architecture-decision-record
