using System.Net;
using System.Net.Http.Json;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Application.Customers;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.IntegrationTests.Customers;

public sealed class CreateCustomerTests(InvoiceTrackerApiFactory factory)
{
    private const string ValidCustomerName = "Test Customer";

    private const string ValidCustomerEmail = "test@example.com";

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task PostCustomer_ReturnsValidationProblem400_WhenCustomerNameIsMissingOrEmpty(string? customerName)
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("api/customers",
            new { Name = customerName, Email = ValidCustomerEmail },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem =
            await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.True(problem.Extensions.ContainsKey("traceId"));
        Assert.Equal(["'Name' must not be empty."], problem.Errors["Name"]);
    }

    [Fact]
    public async Task PostCustomer_Returns201WithNewCustomerLocation_WhenRequestIsValid()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("api/customers",
            new { Name = ValidCustomerName, Email = ValidCustomerEmail },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var location = response.Headers.Location;
        Assert.NotNull(location);
        var customerResponse =
            await client.GetFromJsonAsync<CustomerResponse>(location, TestContext.Current.CancellationToken);
        Assert.NotNull(customerResponse);
        Assert.Equal($"/api/customers/{customerResponse.Id}", location.AbsolutePath);
        Assert.True(customerResponse.Id > 0);
        Assert.Equal(ValidCustomerName, customerResponse.Name);
        Assert.Equal(ValidCustomerEmail, customerResponse.Email);
    }
}
