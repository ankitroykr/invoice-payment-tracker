using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Application.Invoices;

using Microsoft.Extensions.DependencyInjection;

namespace InvoiceTracker.Api.IntegrationTests.Invoices;

public sealed class InvoiceNumberCounterTests(InvoiceTrackerApiFactory factory)
{
    private const int Year = 2085;

    [Fact]
    public async Task GetNextAsync_ReturnsPreviousPlusOne_WhenCalledAgainForTheSameYear()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var invoiceNumberCounter = scope.ServiceProvider.GetRequiredService<IInvoiceNumberCounter>();
        var firstNumber = await invoiceNumberCounter.GetNextAsync(Year, TestContext.Current.CancellationToken);

        // Act
        var secondNumber = await invoiceNumberCounter.GetNextAsync(Year, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(firstNumber + 1, secondNumber);
    }

    [Fact]
    public async Task GetNextAsync_ReturnsOne_WhenYearHasNoNumbersYet()
    {
        // Arrange
        const int year = 2086;
        await using var scope = factory.Services.CreateAsyncScope();
        var invoiceNumberCounter = scope.ServiceProvider.GetRequiredService<IInvoiceNumberCounter>();

        // Act
        var number = await invoiceNumberCounter.GetNextAsync(year, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, number);
    }

    [Fact]
    public async Task GetNextAsync_ReturnsUniqueNumbersWithNoGaps_WhenCalledConcurrentlyForANewYear()
    {
        // Arrange
        const int year = 2087;
        const int callCount = 20;

        // Act
        var numbers = await Task.WhenAll(
            Enumerable.Range(0, callCount).Select(_ => GetNextInOwnScopeAsync(year)));

        // Assert
        Assert.Equal(Enumerable.Range(1, callCount), numbers.Order());
    }

    // Each call gets its own scope, so it runs on its own DbContext and connection,
    // the same as concurrent requests to the API.
    private async Task<int> GetNextInOwnScopeAsync(int year)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var invoiceNumberCounter = scope.ServiceProvider.GetRequiredService<IInvoiceNumberCounter>();

        return await invoiceNumberCounter.GetNextAsync(year, TestContext.Current.CancellationToken);
    }
}
