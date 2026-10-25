# Architecture decision records

The significant decisions behind this project, one short file each, in [MADR](https://adr.github.io/madr/) format (see [0001](0001-record-architecture-decisions.md)). Accepted records aren't edited; a changed decision gets a new record that supersedes the old one.

| ADR | Decision | Status |
|---|---|---|
| [0001](0001-record-architecture-decisions.md) | Record architecture decisions as MADR files in the repository | Accepted |
| [0002](0002-api-style-by-area.md) | Use controllers and minimal APIs, split by area | Accepted |
| [0003](0003-project-layout.md) | Keep source and tests in separate folders under one solution at the root | Accepted |
| [0004](0004-local-orchestration.md) | Run SQL Server locally with Docker Compose, moving to .NET Aspire in Phase 2 | Accepted |
| [0005](0005-xunit-v3-on-microsoft-testing-platform.md) | Use xUnit v3 on Microsoft Testing Platform | Accepted |
| [0006](0006-integration-tests-against-real-sql-server.md) | Run integration tests against a real SQL Server with Testcontainers | Accepted |
| [0007](0007-clean-architecture-layers.md) | Organise the solution in Clean Architecture layers | Accepted |
| [0008](0008-rich-domain-model.md) | Use a rich domain model where there are business rules | Accepted |
| [0009](0009-money-value-object.md) | Represent money as a value object with amount and currency | Accepted |
| [0010](0010-result-type-for-business-failures.md) | Return a Result for expected business failures | Accepted |
