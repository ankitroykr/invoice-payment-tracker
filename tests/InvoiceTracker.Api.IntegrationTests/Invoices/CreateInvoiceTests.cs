using System.Globalization;
using System.Net;
using System.Net.Http.Json;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Api.Security;
using InvoiceTracker.Application.Customers;
using InvoiceTracker.Application.Invoices;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.IntegrationTests.Invoices;

public sealed class CreateInvoiceTests(InvoiceTrackerApiFactory factory)
{
    private const string ValidCustomerName = "Test Customer";

    private const string ValidCustomerEmail = "tester@example.com";

    private const string Currency = "GBP";

    private const string UnsupportedCurrency = "CHF";

    private const int CustomerIdThatDoesNotExist = int.MaxValue;

    [Fact]
    public async Task PostInvoice_Returns201WithNewInvoiceLocation_WhenRequestIsValid()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);
        var customerId = await CreateCustomerAsync(client);

        // Act
        var response = await client.PostAsJsonAsync("api/invoices",
            new
            {
                CustomerId = customerId,
                Currency,
                Lines = new[]
                {
                    new { Description = "Website update and redesign", Quantity = 4m, UnitPrice = 55m }
                }
            },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var location = response.Headers.Location;
        Assert.NotNull(location);
        var invoice = await client.GetFromJsonAsync<InvoiceResponse>(location, TestContext.Current.CancellationToken);
        Assert.NotNull(invoice);
        Assert.Equal($"/api/invoices/{invoice.Id}", location.AbsolutePath);
        Assert.Equal(customerId, invoice.CustomerId);
        Assert.Null(invoice.Number);
        Assert.Equal("Draft", invoice.Status);
        Assert.Equal(Currency, invoice.Currency);
        Assert.Null(invoice.IssueDate);
        Assert.Null(invoice.DueDate);
        // Decimal equality ignores trailing zeros, so the text checks the currency's decimal places too.
        Assert.Equal("220.00", invoice.Total.ToString(CultureInfo.InvariantCulture));
        var line = Assert.Single(invoice.Lines);
        Assert.Equal(new InvoiceLineResponse("Website update and redesign", 4m, 55m, 220m), line);
    }

    [Fact]
    public async Task PostInvoice_ReturnsProblemDetails422_WhenCustomerDoesNotExist()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);

        // Act
        var response = await client.PostAsJsonAsync("api/invoices",
            new
            {
                CustomerId = CustomerIdThatDoesNotExist,
                Currency,
                Lines = new[] { new { Description = "Sr. Engineer Onsite", Quantity = 4m, UnitPrice = 55m } }
            },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.Extensions.ContainsKey("traceId"));
        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, problemDetails.Status);
        Assert.Equal("The customer for this invoice does not exist.", problemDetails.Detail);
    }

    [Fact]
    public async Task PostInvoice_ReturnsProblemDetails422_WhenCurrencyIsNotSupported()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);
        var customerId = await CreateCustomerAsync(client);

        // Act
        var response = await client.PostAsJsonAsync("api/invoices",
            new
            {
                CustomerId = customerId,
                Currency = UnsupportedCurrency,
                Lines = new[] { new { Description = "Sr. Engineer Onsite", Quantity = 4m, UnitPrice = 55m } }
            },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal("The currency for this invoice is not supported.", problemDetails.Detail);
    }

    [Fact]
    public async Task PostInvoice_ReturnsValidationProblem400_WhenInvoiceHasNoLines()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);
        var customerId = await CreateCustomerAsync(client);

        // Act
        var response = await client.PostAsJsonAsync("api/invoices",
            new { CustomerId = customerId, Currency, Lines = Array.Empty<object>() },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem =
            await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.True(problem.Extensions.ContainsKey("traceId"));
        Assert.Equal(["'Lines' must not be empty."], problem.Errors["Lines"]);
    }

    private static async Task<int> CreateCustomerAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("api/customers",
            new { Name = ValidCustomerName, Email = ValidCustomerEmail },
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        var customer = await client.GetFromJsonAsync<CustomerResponse>(response.Headers.Location,
            TestContext.Current.CancellationToken);

        return customer!.Id;
    }
}
