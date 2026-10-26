---
status: accepted
date: 2026-10-21
decision-makers: Ankit Kumar Roy
---

# Keep source and tests in separate folders under one solution at the root

## Context and Problem Statement

The project started from the Web API template, with one project in the root folder. Tests were next, then a domain project, a worker and an Angular app later on. I wanted a layout that would still make sense with several projects.

How should the repository be organised?

## Decision Drivers

* It should be obvious where app code and tests live.
* One command from the root should build and test everything, locally and in CI.
* Everyone should build with the same SDK and the same code style.
* Don't create projects before there's code to put in them.

## Considered Options

* Keep the template layout
* `src/` and `tests/` with one solution at the root
* Full Clean Architecture split from day one

## Decision Outcome

Chosen option: "`src/` and `tests/` with one solution at the root", because it's the layout most .NET repos use and it grows one project at a time.

* App projects in `src/`, test projects in `tests/`, `InvoiceTracker.sln` at the root.
* `global.json` pins the SDK and sets Microsoft Testing Platform as the test runner.
* `.editorconfig` holds shared formatting rules.
* New projects get added when a feature needs them, starting with a domain project.

### Consequences

* Good, because it's familiar and CI only needs the one solution file.
* Good, because SDK updates are a one-line change in `global.json`.
* Bad, because project references use relative paths that break if projects move.

The solution is still a classic `.sln`. Moving to `.slnx`, `Directory.Build.props` and central package management is planned as a separate change later.

### Confirmation

CI builds and tests `InvoiceTracker.sln`, so a project left out of it gets noticed.

## Pros and Cons of the Options

### Keep the template layout

* Good, because there's nothing to change.
* Bad, because tests and new projects have nowhere obvious to go.

### `src/` and `tests/` with one solution at the root

* Good, because it's the usual convention and needs no explanation.
* Neutral, because it meant restructuring the template project early on.

### Full Clean Architecture split from day one

* Good, because project references enforce the layer boundaries.
* Bad, because it means several near-empty projects before there's any domain logic.

## More Information

* Introduced in PR #3.
* `global.json`: https://learn.microsoft.com/en-us/dotnet/core/tools/global-json
