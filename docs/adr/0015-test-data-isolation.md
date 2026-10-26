---
status: accepted
date: 2026-10-26
decision-makers: Ankit Kumar Roy
---

# Reset the test database with Respawn before each integration test

## Context and Problem Statement

All integration tests share one API host and one database (#12). That was fine while they only read. Now tests create customers, so one test's rows can turn up in another, and list and balance tests need exact numbers.

How do I keep tests that write data apart?

## Considered Options

* Unique data per test, no cleanup
* Reset the database with Respawn before each test
* A transaction per test, rolled back
* A database per test class

## Decision Outcome

Chosen option: "Reset the database with Respawn", because every test starts empty and can check exact results.

* Respawn deletes all rows but keeps the tables and the migrations history.
* Integration tests run one at a time. Unit tests still run in parallel.

### Consequences

* Good, because counts and balances can be checked exactly.
* Bad, because the integration tests get slower as they grow. I'll look at this again if it hurts.

## Pros and Cons of the Options

### Unique data per test

* Bad, because data builds up, so list tests can only check "at least".

### Transaction per test

* Bad, because the tests call the API over HTTP, and the API opens its own `DbContext`, so the test can't hold the transaction.

### Database per test class

* Bad, because each class would need its own API host, which brings back the logger problem from #11.

## More Information

* https://github.com/jbogard/Respawn
* https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database
