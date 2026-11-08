using System.Net;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Api.Security;

namespace InvoiceTracker.Api.IntegrationTests.Security;

public sealed class AuthorizationTests(InvoiceTrackerApiFactory factory)
{
    private const int IdThatDoesNotExist = int.MaxValue;

    public static TheoryData<string, string, string[]> RequestsWithoutRequiredRole => new()
    {
        { "GET", $"api/customers/{IdThatDoesNotExist}", [] },
        { "POST", "api/customers", [] },
        { "POST", "api/customers", [AppRoles.Read] },
        { "GET", $"api/invoices/{IdThatDoesNotExist}", [] },
        { "POST", "api/invoices", [] },
        { "POST", "api/invoices", [AppRoles.Read] },
    };

    [Theory]
    [MemberData(nameof(RequestsWithoutRequiredRole))]
    public async Task Request_ReturnsForbidden_WhenTokenLacksRequiredRole(string method, string path, string[] roles)
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(roles);
        using var request = new HttpRequestMessage(new HttpMethod(method), path);

        // Act
        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
