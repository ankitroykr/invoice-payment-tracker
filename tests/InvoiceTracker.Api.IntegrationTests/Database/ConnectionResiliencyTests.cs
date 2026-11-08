using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Infrastructure.Data;

using Microsoft.Extensions.DependencyInjection;

namespace InvoiceTracker.Api.IntegrationTests.Database;

public sealed class ConnectionResiliencyTests(InvoiceTrackerApiFactory factory)
{
    [Fact]
    public async Task ExecutionStrategy_RetriesOnFailure_WhenUsingSqlServer()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InvoiceTrackerDbContext>();

        // Act
        var strategy = dbContext.Database.CreateExecutionStrategy();

        // Assert
        Assert.True(strategy.RetriesOnFailure);
    }
}
