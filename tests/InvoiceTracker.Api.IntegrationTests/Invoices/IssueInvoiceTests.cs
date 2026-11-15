using System.Net;
using System.Net.Http.Json;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Api.Security;
using InvoiceTracker.Application.Invoices;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.IntegrationTests.Invoices;

public sealed class IssueInvoiceTests(InvoiceTrackerApiFactory factory)
{
    private const int InvoiceIdThatDoesNotExist = int.MaxValue;

    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    [Fact]
    public async Task PostIssue_Returns204AndIssuesInvoice_WhenInvoiceIsDraft()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);
        var invoiceId = await client.CreateInvoiceAsync();
        var dueDate = Today.AddDays(30);

        // Act
        var response = await client.PostAsJsonAsync($"api/invoices/{invoiceId}/issue", new { DueDate = dueDate },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var invoice = await client.GetFromJsonAsync<InvoiceResponse>($"api/invoices/{invoiceId}",
            TestContext.Current.CancellationToken);
        Assert.NotNull(invoice);
        Assert.Equal("Issued", invoice.Status);
        Assert.Equal(Today, invoice.IssueDate);
        Assert.Equal(dueDate, invoice.DueDate);
        Assert.Matches($$"""^INV-{{Today.Year}}-\d{4,}$""", invoice.Number);
    }

    [Fact]
    public async Task PostIssue_ReturnsValidationProblem400_WhenDueDateIsBeforeToday()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);
        var invoiceId = await client.CreateInvoiceAsync();

        // Act
        var response = await client.PostAsJsonAsync($"api/invoices/{invoiceId}/issue",
            new { DueDate = Today.AddDays(-1) },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem =
            await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.True(problem.Errors.ContainsKey("DueDate"));
    }

    [Fact]
    public async Task PostIssue_ReturnsProblemDetails404_WhenInvoiceDoesNotExist()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);

        // Act
        var response = await client.PostAsJsonAsync($"api/invoices/{InvoiceIdThatDoesNotExist}/issue",
            new { DueDate = Today },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.Equal("The invoice was not found.", problem.Detail);
    }

    [Fact]
    public async Task PostIssue_ReturnsProblemDetails422_WhenInvoiceIsAlreadyIssued()
    {
        // Arrange
        var client = factory.CreateAuthenticatedClient(AppRoles.ReadWrite);
        var invoiceId = await client.CreateInvoiceAsync();
        var firstIssue = await client.PostAsJsonAsync($"api/invoices/{invoiceId}/issue", new { DueDate = Today },
            TestContext.Current.CancellationToken);
        firstIssue.EnsureSuccessStatusCode();

        // Act
        var response = await client.PostAsJsonAsync($"api/invoices/{invoiceId}/issue", new { DueDate = Today },
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.Equal("Only a draft invoice can be changed or issued.", problem.Detail);
    }
}
