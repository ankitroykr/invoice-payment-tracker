using System.Text.Json;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;

namespace InvoiceTracker.Api.IntegrationTests.OpenApi;

public sealed class OpenApiDocumentTests(InvoiceTrackerApiFactory factory)
{
    [Theory]
    [InlineData("/api/customers", "post", new[] { "201", "400" })]
    [InlineData("/api/customers/{id}", "get", new[] { "200", "404" })]
    [InlineData("/api/invoices", "post", new[] { "201", "400", "422" })]
    [InlineData("/api/invoices/{id}", "get", new[] { "200", "404" })]
    public async Task OpenApiDocument_DocumentsEndpoint_WithSummaryAndStatusCodes(
        string path, string method, string[] expectedStatusCodes)
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var json = await client.GetStringAsync("/openapi/v1.json", TestContext.Current.CancellationToken);

        // Assert
        using var document = JsonDocument.Parse(json);
        var operation = document.RootElement.GetProperty("paths").GetProperty(path).GetProperty(method);

        Assert.True(operation.TryGetProperty("summary", out var summary), $"{method} {path} has no summary.");
        Assert.False(string.IsNullOrWhiteSpace(summary.GetString()));

        var statusCodes = operation.GetProperty("responses").EnumerateObject().Select(response => response.Name);
        Assert.Equal(expectedStatusCodes.Order(), statusCodes.Order());
    }

    [Fact]
    public async Task OpenApiDocument_DefinesBearerSecurityScheme()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var json = await client.GetStringAsync("/openapi/v1.json", TestContext.Current.CancellationToken);

        // Assert
        using var document = JsonDocument.Parse(json);
        var bearerScheme = document.RootElement.GetProperty("components").GetProperty("securitySchemes")
            .GetProperty("Bearer");

        Assert.Equal("http", bearerScheme.GetProperty("type").GetString());
        Assert.Equal("bearer", bearerScheme.GetProperty("scheme").GetString());
        Assert.Equal("JWT", bearerScheme.GetProperty("bearerFormat").GetString());
    }

    [Theory]
    [InlineData("/api/customers", "post")]
    [InlineData("/api/customers/{id}", "get")]
    [InlineData("/api/invoices", "post")]
    [InlineData("/api/invoices/{id}", "get")]
    public async Task OpenApiDocument_RequiresBearerToken_OnApiEndpoints(string path, string method)
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var json = await client.GetStringAsync("/openapi/v1.json", TestContext.Current.CancellationToken);

        // Assert
        using var document = JsonDocument.Parse(json);
        var operation = document.RootElement.GetProperty("paths").GetProperty(path).GetProperty(method);

        Assert.Contains(operation.GetProperty("security").EnumerateArray(),
            requirement => requirement.TryGetProperty("Bearer", out _));
    }
}
