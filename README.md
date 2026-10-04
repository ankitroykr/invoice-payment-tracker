# Invoice and Payment Tracker

A full-stack web application for freelancers to create invoices, record full and partial payments, and track outstanding and overdue balances.

I am building it incrementally, one small feature per pull request, to show end-to-end delivery: API and domain design, SQL Server, automated testing, security, CI/CD and Azure deployment.

## Project status

- ✅ ASP.NET Core API (.NET 10) running locally
- ✅ Liveness and readiness health check endpoints
- ⏭️ Next: restructure the solution into `src/` and `tests/` with shared build settings

## Technology stack

- **Backend:** C#, ASP.NET Core (.NET 10)
- **Frontend:** Angular and TypeScript (planned)
- **Data:** SQL Server with Entity Framework Core (planned)
- **Testing:** xUnit with Testcontainers for integration tests (planned)
- **Delivery:** GitHub Actions, with Azure hosting in a later phase

## Roadmap

- [x] Create and run the initial API
- [x] Add liveness and readiness health checks
- [ ] Project structure, shared build settings and CI build
- [ ] SQL Server via Docker and Entity Framework Core
- [ ] Customers and invoices API
- [ ] Payment recording with idempotency and concurrency control
- [ ] Angular website
- [ ] Authentication and per-user data isolation
- [ ] Azure deployment, background reminders and monitoring

## Running the application

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- JetBrains Rider or VS Code
- A trusted HTTPS development certificate:

  ```bash
  dotnet dev-certs https --trust
  ```

### Run the API

From the repository root:

```bash
dotnet run --project InvoiceTracker/InvoiceTracker.Api --launch-profile https
```

Or, in Rider, select the **InvoiceTracker.Api: https** run configuration.

The API listens on `https://localhost:7160`.

### Health checks

| Endpoint | Purpose |
|---|---|
| `GET /health/live` | Liveness: the process is running and responding. Runs no dependency checks. |
| `GET /health/ready` | Readiness: the API's dependencies are available. Runs checks tagged `ready`. |

Both return JSON, for example:

```json
{ "status": "Healthy", "checks": [] }
```

Example requests are in [`InvoiceTracker.Api.http`](InvoiceTracker/InvoiceTracker.Api/InvoiceTracker.Api.http) and can be run from Rider or VS Code.

## Licence

This project is licensed under the [MIT License](LICENSE).
