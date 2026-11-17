using System.Net;
using System.Net.Http.Json;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Api.Security;
using InvoiceTracker.Application.Invoices;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.IntegrationTests.Invoices;

public sealed class VoidInvoiceTests(InvoiceTrackerApiFactory factory)
{
    private const int InvoiceIdThatDoesNotExist = int.MaxValue;

    private const int InvalidInvoiceId = 0;

    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    [Fact]
    public async Task PostVoid_Returns204AndVoidsInvoice_WhenInvoiceIsIssued()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);
        var invoiceId = await client.CreateInvoiceAsync();
        var dueDate = Today.AddDays(30);

        var issueResponse = await client.PostAsJsonAsync($"api/invoices/{invoiceId}/issue", new { DueDate = dueDate },
            TestContext.Current.CancellationToken);
        issueResponse.EnsureSuccessStatusCode();

        // Act
        var voidResponse =
            await client.PostAsync($"api/invoices/{invoiceId}/void", null, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, voidResponse.StatusCode);
        var invoice = await client.GetFromJsonAsync<InvoiceResponse>($"api/invoices/{invoiceId}",
            TestContext.Current.CancellationToken);
        Assert.NotNull(invoice);
        Assert.Equal("Void", invoice.Status);
    }

    [Fact]
    public async Task PostVoid_ReturnsProblemDetails422_WhenInvoiceIsStillDraft()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);
        var invoiceId = await client.CreateInvoiceAsync();

        // Act
        var response =
            await client.PostAsync($"api/invoices/{invoiceId}/void", null, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.Equal("Only an issued invoice can be voided.", problem.Detail);
    }

    [Fact]
    public async Task PostVoid_ReturnsProblemDetails422_WhenInvoiceIsAlreadyVoid()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);
        var invoiceId = await client.CreateInvoiceAsync();
        var issueResponse = await client.PostAsJsonAsync($"api/invoices/{invoiceId}/issue", new { DueDate = Today },
            TestContext.Current.CancellationToken);
        issueResponse.EnsureSuccessStatusCode();
        var firstVoid =
            await client.PostAsync($"api/invoices/{invoiceId}/void", null, TestContext.Current.CancellationToken);
        firstVoid.EnsureSuccessStatusCode();

        // Act
        var response =
            await client.PostAsync($"api/invoices/{invoiceId}/void", null, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.Equal("Only an issued invoice can be voided.", problem.Detail);
    }

    [Fact]
    public async Task PostVoid_ReturnsProblemDetails404_WhenInvoiceDoesNotExist()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);

        // Act
        var response = await client.PostAsync($"api/invoices/{InvoiceIdThatDoesNotExist}/void", null,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.Equal("The invoice was not found.", problem.Detail);
    }

    [Fact]
    public async Task PostVoid_ReturnsValidationProblem400_WhenInvoiceIdIsNotPositive()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);

        // Act
        var response = await client.PostAsync($"api/invoices/{InvalidInvoiceId}/void", null,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem =
            await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey("InvoiceId"));
    }
}
