# Invoice and Payment Tracker

A personal project to build an invoice and payment tracker from scratch, the way I would at work: in small pull requests, with CI checking every change.

When complete, the application will let freelancers create invoices, record full and partial payments, and keep track of outstanding and overdue balances. It is built with ASP.NET Core and SQL Server, with an Angular front end and Azure hosting planned.

## Roadmap

- [x] Create and run the initial API
- [x] Liveness and readiness health checks
- [x] Project structure, SDK pinning and editor settings
- [x] CI build with GitHub Actions, with `main` protected by required checks
- [x] SQL Server via Docker Compose, EF Core migrations and a database readiness check
- [ ] Customers and invoices API
- [ ] Payment recording with idempotency and concurrency control
- [ ] Angular website
- [ ] Authentication and per-user data isolation
- [ ] Azure deployment, background reminders and monitoring

## Technology stack

- **Backend:** C#, ASP.NET Core (.NET 10)
- **Data:** SQL Server 2022 (Docker locally) with Entity Framework Core 10
- **Delivery:** GitHub Actions, with Azure hosting in a later phase
- **Frontend:** Angular and TypeScript (planned)
- **Testing:** xUnit with Testcontainers for integration tests (planned)

## Repository layout

```
├── .github/workflows/ci.yml   CI workflow
├── src/InvoiceTracker.Api/    ASP.NET Core API, EF Core DbContext and migrations
├── compose.yaml               Local SQL Server
├── .env.example               Template for the local .env file
├── global.json                Pinned .NET SDK version
├── .editorconfig              Shared formatting and code style rules
└── InvoiceTracker.sln         Solution file
```

## Running the application

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (10.0.401 or later; pinned in `global.json`)
- JetBrains Rider or VS Code
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- The EF Core CLI: `dotnet tool install --global dotnet-ef`
- A trusted HTTPS development certificate:

  ```bash
  dotnet dev-certs https --trust
  ```

### Start SQL Server

1. Copy [`.env.example`](.env.example) to `.env` and set a strong `MSSQL_SA_PASSWORD` (at least 8 characters, from three of: upper case, lower case, digits, symbols). `.env` is git-ignored.
2. Start the database defined in [`compose.yaml`](compose.yaml):

   ```bash
   docker compose up -d
   ```

3. Wait until `docker compose ps` shows the container as `healthy`.

To stop it, run `docker compose stop` (start it again with `docker compose start`). `docker compose down` removes the container but keeps the data volume; `docker compose down -v` also deletes the data.

### Configure the connection string

The API reads `ConnectionStrings:InvoiceTrackerDb` from configuration. For local development, store it with the Secret Manager so it never enters the repository:

```bash
dotnet user-secrets set "ConnectionStrings:InvoiceTrackerDb" "Server=localhost,1433;Database=InvoiceTracker;User Id=sa;Password=<your MSSQL_SA_PASSWORD>;TrustServerCertificate=True" --project src/InvoiceTracker.Api
```

### Create the database

```bash
dotnet ef database update --project src/InvoiceTracker.Api
```

### Run the API

From the repository root:

```bash
dotnet run --project src/InvoiceTracker.Api --launch-profile https
```

Or open `InvoiceTracker.sln` in Rider and select the **InvoiceTracker.Api: https** run configuration.

The API listens on `https://localhost:7160`.

### Health checks

| Endpoint | Purpose |
|---|---|
| `GET /health/live` | Liveness: the process is running and responding. Runs no dependency checks. |
| `GET /health/ready` | Readiness: the API's dependencies are available. Runs the checks tagged `ready` (currently the SQL Server connection) and returns `503` when the database is unreachable. |

Both return JSON. For example, from `/health/ready`:

```json
{ "status": "Healthy", "checks": [ { "name": "database", "status": "Healthy", "description": null } ] }
```

Example requests are in [`InvoiceTracker.Api.http`](src/InvoiceTracker.Api/InvoiceTracker.Api.http) and can be run from Rider or VS Code.

## Continuous integration

Every pull request to `main`, and every push to `main`, runs the [CI workflow](.github/workflows/ci.yml) on GitHub Actions. It restores, builds and tests the solution in Release, using the SDK pinned in `global.json`. Pull requests can only be merged once the `build` check passes.

The workflow is based on GitHub's official [.NET starter workflow](https://github.com/actions/starter-workflows/blob/main/ci/dotnet.yml). I updated the action versions, read the SDK version from `global.json`, and added read-only permissions and cancellation of outdated runs.

Runs are listed under the repository's [Actions tab](https://github.com/ankitroykr/invoice-payment-tracker/actions).

## References

- [Health checks in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [Run SQL Server Linux containers with Docker](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker)
- [Compose file reference](https://docs.docker.com/reference/compose-file/)
- [EF Core migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Safe storage of app secrets in development](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [GitHub Actions workflow syntax](https://docs.github.com/en/actions/reference/workflows-and-actions/workflow-syntax)

## Licence

This project is licensed under the [MIT License](LICENSE).
