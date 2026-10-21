---
status: accepted
date: 2026-10-21
decision-makers: Ankit Kumar Roy
---

# Use xUnit v3 on Microsoft Testing Platform

## Context and Problem Statement

Most codebases I've worked on use xUnit v2 with VSTest, the runner behind the classic `dotnet test`. Since then xUnit v3 has shipped, and Microsoft has introduced Microsoft Testing Platform (MTP), where each test project runs as its own executable.

Which test framework and runner should the project use?

## Decision Drivers

* The framework should still be actively developed.
* Tests should run the same way in Rider, from the terminal and in CI.
* Integration tests need shared setup, because they start a SQL Server container.

## Considered Options

* xUnit v2 with VSTest
* xUnit v3 with MTP
* NUnit
* MSTest

## Decision Outcome

Chosen option: "xUnit v3 with MTP", because v2 is in maintenance mode and v3 has features I'm already using, such as assembly fixtures.

* The test project references `xunit.v3.mtp-v2` and builds as an executable.
* `global.json` sets MTP as the runner for `dotnet test`.
* Locally and in CI, tests run with `dotnet test --solution InvoiceTracker.sln`.

### Consequences

* Good, because assembly fixtures let all test classes share one SQL container. That fixed the flaky CI failure in issue #11.
* Bad, because it's newer, so there are fewer examples and some `dotnet test` options differ.
* Bad, because output from passing tests isn't shown by default, which makes some debugging harder.

### Confirmation

CI runs the same `dotnet test --solution` command as I do locally.

## Pros and Cons of the Options

### xUnit v2 with VSTest

* Good, because it's what most existing codebases use.
* Bad, because xUnit's own site says v2 only gets critical fixes, with no new features.

### xUnit v3 with MTP

* Good, because it's actively developed and MTP is where `dotnet test` is heading.
* Bad, because the tooling is still catching up.

### NUnit

* Good, because it's mature, with rich assertions.
* Neutral, because it would work, but it's less common in the codebases I work on.

### MSTest

* Good, because it's Microsoft's own and supports MTP well.
* Bad, because I rarely see it in enterprise or open-source code.

## More Information

* Introduced in PR #6; assembly fixture added in PR #12.
* xUnit v3: https://xunit.net/docs/getting-started/v3/whats-new
* Microsoft Testing Platform: https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-intro
