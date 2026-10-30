# Architecture decision records

The significant decisions behind this project, one short file each, in [MADR](https://adr.github.io/madr/) format (see [0001](0001-record-architecture-decisions.md)). Accepted records aren't edited; a changed decision gets a new record that supersedes the old one.

| ADR | Decision | Status |
|---|---|---|
| [0001](0001-record-architecture-decisions.md) | Record architecture decisions as MADR files in the repository | Accepted |
| [0002](0002-api-style-by-area.md) | Use controllers and minimal APIs, split by area | Accepted |
| [0003](0003-project-layout.md) | Keep source and tests in separate folders under one solution at the root | Accepted |
| [0004](0004-local-orchestration.md) | Run SQL Server locally with Docker Compose, moving to .NET Aspire later | Accepted |
| [0005](0005-xunit-v3-on-microsoft-testing-platform.md) | Use xUnit v3 on Microsoft Testing Platform | Accepted |
| [0006](0006-integration-tests-against-real-sql-server.md) | Run integration tests against a real SQL Server with Testcontainers | Accepted |
| [0007](0007-clean-architecture-layers.md) | Organise the solution in Clean Architecture layers | Accepted |
| [0008](0008-rich-domain-model.md) | Use a rich domain model where there are business rules | Accepted |
| [0009](0009-money-value-object.md) | Represent money as a value object with amount and currency | Accepted |
| [0010](0010-result-type-for-business-failures.md) | Return a Result for expected business failures | Accepted |
| [0011](0011-repositories-and-unit-of-work.md) | Use a generic repository per aggregate root with a unit of work | Accepted |
| [0012](0012-request-validation.md) | Validate requests with FluentValidation in the Application layer | Accepted |
| [0013](0013-plain-command-handlers.md) | Call use cases through plain command handlers | Accepted |
| [0014](0014-query-side.md) | Read data through query interfaces, using EF Core first | Accepted |
| [0015](0015-test-data-isolation.md) | Reset the test database with Respawn before each integration test | Accepted |
| [0016](0016-error-to-problem-details.md) | Map errors to Problem Details by error type | Accepted |
