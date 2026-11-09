---
status: accepted
date: 2026-11-09
decision-makers: Ankit Kumar Roy
---

# Host the API on the App Service and Azure SQL free tiers

## Context and Problem Statement

The API needs to run somewhere public, against a real SQL database, so it can be called and checked. It has no real users yet, so hosting must cost nothing. No password or key should be stored in the repository, in GitHub or in the app's settings.

Where does the API run, and how does it get there?

## Considered Options

* App Service Free (F1) on Linux, with the Azure SQL Database free offer
* Azure Container Apps, with the Azure SQL Database free offer
* A virtual machine running the API and SQL Server

## Decision Outcome

Chosen option: "App Service Free (F1) on Linux, with the Azure SQL Database free offer", because it runs the published .NET app as it is, with no container image or registry, and both have a free tier that doesn't expire.

* The app signs in to Azure SQL with its system-assigned managed identity, so the connection string has no password. Its database user can only read and write rows.
* The SQL server only accepts Microsoft Entra sign-ins, and its firewall only allows the app's outbound IP addresses. F1 has no virtual network integration, so a firewall is the closest option.
* GitHub Actions deploys after CI passes on `main`. It signs in with OIDC as a user-assigned managed identity, trusted only for the `production` environment, which only `main` can deploy to.
* The deploy workflow applies the EF Core migrations with a migration bundle before the new code goes live. It opens the SQL firewall to the runner's IP address for that run only. The app doesn't migrate at startup, so it never needs rights to change tables.
* The database pauses itself if the monthly free allowance runs out, rather than billing.

### Consequences

* Good, because it costs nothing and no secret is stored anywhere.
* Good, because each identity has only the rights it needs.
* Bad, because F1 gives 60 CPU minutes a day, has no Always On and no deployment slots, so the first request after a quiet spell is slow and a deploy briefly takes the app down.
* Bad, because the free database pauses when it isn't used, so the first request after that waits for it to resume.
* Bad, because the firewall rules follow the app's outbound IP addresses, which change if the app moves to another plan.

## Pros and Cons of the Options

### Azure Container Apps

* Good, because it scales to zero and has a monthly free grant.
* Bad, because it needs a container image and a registry, which is more to set up and maintain for one API.

### A virtual machine

* Bad, because the free virtual machine offer only lasts 12 months, and patching the machine and SQL Server would be my job.

## More Information

* https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/azure-subscription-service-limits#azure-app-service-limits
* https://learn.microsoft.com/en-us/azure/azure-sql/database/free-offer
* https://learn.microsoft.com/en-us/azure/app-service/tutorial-connect-msi-sql-database
* https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure-openid-connect
* https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying#bundles
