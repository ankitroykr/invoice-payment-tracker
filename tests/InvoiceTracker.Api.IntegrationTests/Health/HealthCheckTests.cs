using System.Net;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;

namespace InvoiceTracker.Api.IntegrationTests.Health;

public class HealthCheckTests(InvoiceTrackerApiFactory factory)
{
    [Fact]
    public async Task GetReady_ReturnsOk_WhenDatabaseIsAvailable()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/ready", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
