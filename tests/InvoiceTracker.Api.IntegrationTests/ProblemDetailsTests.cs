using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.IntegrationTests;

public class ProblemDetailsTests(InvoiceTrackerApiFactory factory)
{
    [Fact]
    public async Task GetUnknownRoute_ReturnsProblemDetails404()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/unknown", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.True(problem.Extensions.ContainsKey("traceId"));
        Assert.Equal((int)HttpStatusCode.NotFound, problem.Status);
    }
}
