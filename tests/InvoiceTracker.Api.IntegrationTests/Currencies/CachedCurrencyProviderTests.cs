using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Application.Currencies;
using InvoiceTracker.Infrastructure.Currencies;
using InvoiceTracker.Infrastructure.Data;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceTracker.Api.IntegrationTests.Currencies;

public sealed class CachedCurrencyProviderTests(InvoiceTrackerApiFactory factory)
{
    [Fact]
    public async Task FindAsync_ReturnsCurrency_WhenCurrencyIsSupported()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var currencyProvider = scope.ServiceProvider.GetRequiredService<ICurrencyProvider>();

        // Act
        var currency = await currencyProvider.FindAsync("GBP", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(new SupportedCurrency("GBP", 2, "Pound Sterling"), currency);
    }

    [Fact]
    public async Task FindAsync_ReturnsNull_WhenCurrencyIsNotSupported()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var currencyProvider = scope.ServiceProvider.GetRequiredService<ICurrencyProvider>();

        // Act
        var currency = await currencyProvider.FindAsync("JPY", TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(currency);
    }

    [Fact]
    public async Task FindAsync_DoesNotQueryDatabase_WhenCurrenciesAreCached()
    {
        // Arrange
        var scope = factory.Services.CreateAsyncScope();
        var currencyProvider = new CachedCurrencyProvider(
            scope.ServiceProvider.GetRequiredService<InvoiceTrackerDbContext>(),
            factory.Services.GetRequiredService<HybridCache>());
        await currencyProvider.FindAsync("GBP", TestContext.Current.CancellationToken);

        // Disposing the scope disposes the DbContext, so a database query would now throw.
        await scope.DisposeAsync();

        // Act
        var currency = await currencyProvider.FindAsync("EUR", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(new SupportedCurrency("EUR", 2, "Euro"), currency);
    }
}
