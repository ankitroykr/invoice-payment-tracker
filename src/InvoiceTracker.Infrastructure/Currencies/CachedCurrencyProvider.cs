using InvoiceTracker.Application.Currencies;
using InvoiceTracker.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace InvoiceTracker.Infrastructure.Currencies;

public sealed class CachedCurrencyProvider(InvoiceTrackerDbContext dbContext, HybridCache cache) : ICurrencyProvider
{
    private const string CacheKey = "currencies";

    // Currencies only change with a migration, and a deployment restarts the app, which empties the cache.
    // The hour only matters if someone changes a row by hand.
    private static readonly HybridCacheEntryOptions CacheOptions = new()
    {
        Expiration = TimeSpan.FromHours(1), LocalCacheExpiration = TimeSpan.FromHours(1)
    };

    public async Task<SupportedCurrency?> FindAsync(string code, CancellationToken cancellationToken = default)
    {
        var currencies = await cache.GetOrCreateAsync(
            CacheKey,
            async token => await dbContext.Currencies
                .AsNoTracking()
                .Select(c => new SupportedCurrency(c.Code, c.MinorUnits, c.Name))
                .ToListAsync(token),
            CacheOptions,
            cancellationToken: cancellationToken
        );

        return currencies.SingleOrDefault(c => c.Code == code);
    }
}
