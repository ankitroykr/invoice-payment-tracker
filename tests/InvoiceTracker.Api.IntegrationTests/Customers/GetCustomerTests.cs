using System.Net;
using System.Net.Http.Json;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.IntegrationTests.Customers;

public sealed class GetCustomerTests(InvoiceTrackerApiFactory factory)
{
    private const int CustomerIdThatDoesNotExist = int.MaxValue;

    [Fact]
    public async Task GetCustomer_ReturnsProblemDetails404_WhenCustomerDoesNotExist()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync($"api/customers/{CustomerIdThatDoesNotExist}",
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
