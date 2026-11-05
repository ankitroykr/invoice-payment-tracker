using System.Net;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;

namespace InvoiceTracker.Api.IntegrationTests.Security;

public sealed class AuthenticationTests(InvoiceTrackerApiFactory factory)
{
    private const int CustomerIdThatDoesNotExist = int.MaxValue;

    public static TheoryData<string, string> ApiEndpoints => new()
    {
        { "POST", "api/customers" },
        { "GET", $"api/customers/{CustomerIdThatDoesNotExist}" },
        { "POST", "api/invoices" },
        { "GET", $"api/invoices/{CustomerIdThatDoesNotExist}" },
    };

    [Theory]
    [MemberData(nameof(ApiEndpoints))]
    public async Task Request_ReturnsUnauthorizedWithBearerChallenge_WhenNoTokenIsSent(string method, string path)
    {
        // Arrange
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(new HttpMethod(method), path);

        // Act
        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains(response.Headers.WwwAuthenticate, challenge => challenge.Scheme == "Bearer");
    }
}
