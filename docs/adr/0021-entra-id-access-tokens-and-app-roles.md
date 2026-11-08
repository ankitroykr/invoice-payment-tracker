---
status: accepted
date: 2026-11-05
decision-makers: Ankit Kumar Roy
---

# Protect the API with Entra ID access tokens and app roles

## Context and Problem Statement

The API holds customer and invoice data, so only known callers should reach it, and each caller should only do what it's allowed to. The first callers are other apps, using the OAuth 2.0 client credentials flow. An Angular app will sign users in later.

How does the API check who's calling and what they can do?

## Considered Options

* Entra ID access tokens, checked with ASP.NET Core's JwtBearer handler
* Entra ID access tokens, checked with Microsoft.Identity.Web
* API keys

## Decision Outcome

Chosen option: "Entra ID access tokens, checked with ASP.NET Core's JwtBearer handler", because the API only checks tokens and calls no other API, so Microsoft.Identity.Web adds nothing it would use.

* Two app roles for the whole API, `InvoiceTracker.Read` and `InvoiceTracker.ReadWrite`, checked through named policies on the controllers.
* Every endpoint needs a token unless it allows anonymous calls. Only the health checks do.
* The caller's id is the `oid` claim, for apps now and users later.
* Locally the API trusts tokens from `dotnet user-jwts`, so it runs without Azure. Integration tests sign their own.

### Consequences

* Good, because it's standard OAuth 2.0, and Entra ID Free covers it.
* Bad, because if the API ever calls another API for a user, it'll need Microsoft.Identity.Web and a new ADR.

## Pros and Cons of the Options

### Microsoft.Identity.Web

* Good, because the Entra docs recommend it.
* Bad, because most of it is for calling other APIs.

### API keys

* Bad, because a key doesn't say who's calling and never expires.

## More Information

* https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication
* https://learn.microsoft.com/en-us/entra/identity-platform/howto-add-app-roles-in-apps
* https://learn.microsoft.com/en-us/entra/identity-platform/access-token-claims-reference
* https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn
