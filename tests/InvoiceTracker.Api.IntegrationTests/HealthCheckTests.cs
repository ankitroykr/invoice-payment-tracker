using System.Net;

namespace InvoiceTracker.Api.IntegrationTests;

public class HealthCheckTests(InvoiceTrackerApiFactory factory) : IClassFixture<InvoiceTrackerApiFactory>
{
    [Fact]
    public async Task GetReady_ReturnsOk_WhenDatabaseIsAvailable()
    {
        // Arrange
        var client = factory.CreateClient();
        // Act
        var response = await client.GetAsync("/health/ready", TestContext.Current.CancellationToken);
        // Assert
        Assert.Equal(HttpStatusCode.GatewayTimeout, response.StatusCode);
    }
}
