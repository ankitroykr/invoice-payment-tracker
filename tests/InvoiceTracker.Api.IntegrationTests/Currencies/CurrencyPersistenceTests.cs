using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceTracker.Api.IntegrationTests.Currencies;

public sealed class CurrencyPersistenceTests(InvoiceTrackerApiFactory factory)
{
    [Fact]
    public async Task Currencies_ContainsSupportedCurrenciesWithMinorUnits_WhenDatabaseIsMigrated()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InvoiceTrackerDbContext>();

        // Act
        var currencies = await dbContext.Currencies
            .AsNoTracking()
            .OrderBy(c => c.Code)
            .Select(c => new { c.Code, c.MinorUnits, c.Name })
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        var expected = new[]
        {
            new { Code = "EUR", MinorUnits = 2, Name = "Euro" },
            new { Code = "GBP", MinorUnits = 2, Name = "Pound Sterling" },
            new { Code = "INR", MinorUnits = 2, Name = "Indian Rupee" },
            new { Code = "USD", MinorUnits = 2, Name = "US Dollar" }
        };
        Assert.Equal(expected, currencies);
    }
}
