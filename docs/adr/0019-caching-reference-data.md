---
status: accepted
date: 2026-11-03
decision-makers: Ankit Kumar Roy
---

# Cache reference data in memory with HybridCache

## Context and Problem Statement

Every new invoice needs its currency's minor units. Currencies only change with a migration, so reading them from the database each time is wasted work.

How do I cache reference data like currencies?

## Considered Options

* HybridCache, in memory only
* `IMemoryCache`
* HybridCache with Redis
* No cache

## Decision Outcome

Chosen option: "HybridCache, in memory only", because it's the current .NET caching API, and when the cache is empty only one request goes to the database.

* The whole currency list is cached under one key for an hour.
* A deploy restarts the app and clears the cache, so a new currency shows up straight away.
* The cache stores JSON, so it holds a small Application record, not the domain entity.

### Consequences

* Good, because one query serves every invoice for an hour.
* Good, because adding Redis later is only a registration change.
* Bad, because each server has its own cache. Fine while there's one.

## Pros and Cons of the Options

### `IMemoryCache`

* Bad, because requests that miss at the same time all hit the database.

### HybridCache with Redis

* Bad, because Azure Cache for Redis isn't free.

### No cache

* Bad, because every invoice reads the same four rows.

## More Information

* https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid
