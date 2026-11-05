using System.Net;
using System.Net.Http.Json;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Api.Security;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.IntegrationTests.Invoices;

public sealed class GetInvoiceTests(InvoiceTrackerApiFactory factory)
{
    private const int InvoiceIdThatDoesNotExist = int.MaxValue;

    [Fact]
    public async Task GetInvoice_ReturnsProblemDetails404_WhenInvoiceDoesNotExist()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.Read);

        // Act
        var response = await client.GetAsync($"api/invoices/{InvoiceIdThatDoesNotExist}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.Extensions.ContainsKey("traceId"));
        Assert.Equal((int)HttpStatusCode.NotFound, problemDetails.Status);
    }
}
