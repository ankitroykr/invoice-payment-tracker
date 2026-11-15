using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Application.Invoices;

using Microsoft.Extensions.DependencyInjection;

namespace InvoiceTracker.Api.IntegrationTests.Invoices;

public sealed class InvoiceNumberCounterTests(InvoiceTrackerApiFactory factory)
{
    private const int Year = 2085;

    [Fact]
    public async Task GetNextSequenceAsync_ReturnsPreviousPlusOne_WhenCalledAgainForTheSameYear()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var invoiceNumberCounter = scope.ServiceProvider.GetRequiredService<IInvoiceNumberCounter>();
        var firstSequence =
            await invoiceNumberCounter.GetNextSequenceAsync(Year, TestContext.Current.CancellationToken);

        // Act
        var secondSequence =
            await invoiceNumberCounter.GetNextSequenceAsync(Year, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(firstSequence + 1, secondSequence);
    }

    [Fact]
    public async Task GetNextSequenceAsync_ReturnsOne_WhenYearHasNoSequencesYet()
    {
        // Arrange
        const int year = 2086;
        await using var scope = factory.Services.CreateAsyncScope();
        var invoiceNumberCounter = scope.ServiceProvider.GetRequiredService<IInvoiceNumberCounter>();

        // Act
        var sequence = await invoiceNumberCounter.GetNextSequenceAsync(year, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, sequence);
    }

    [Fact]
    public async Task GetNextSequenceAsync_ReturnsUniqueSequencesWithNoGaps_WhenCalledConcurrentlyForANewYear()
    {
        // Arrange
        const int year = 2087;
        const int callCount = 20;

        // Act
        var sequences = await Task.WhenAll(
            Enumerable.Range(0, callCount).Select(_ => GetNextSequenceInOwnScopeAsync(year)));

        // Assert
        Assert.Equal(Enumerable.Range(1, callCount), sequences.Order());
    }

    // Each call gets its own scope, so it runs on its own DbContext and connection,
    // the same as concurrent requests to the API.
    private async Task<int> GetNextSequenceInOwnScopeAsync(int year)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var invoiceNumberCounter = scope.ServiceProvider.GetRequiredService<IInvoiceNumberCounter>();

        return await invoiceNumberCounter.GetNextSequenceAsync(year, TestContext.Current.CancellationToken);
    }
}
