using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;

using Microsoft.IdentityModel.Tokens;

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

    [Fact]
    public async Task Request_ReturnsUnauthorizedWithInvalidTokenError_WhenTokenIsSignedWithAnotherKey()
    {
        // Arrange
        var untrustedKey = new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(64));
        var token = factory.CreateToken(signingKey: untrustedKey);

        // Act
        var response = await SendWithTokenAsync(token);

        // Assert
        AssertInvalidTokenChallenge(response);
    }

    [Fact]
    public async Task Request_ReturnsUnauthorizedWithInvalidTokenError_WhenTokenIsForAnotherAudience()
    {
        // Arrange
        var token = factory.CreateToken(audience: "api://another-api");

        // Act
        var response = await SendWithTokenAsync(token);

        // Assert
        AssertInvalidTokenChallenge(response);
    }

    [Fact]
    public async Task Request_ReturnsUnauthorizedWithInvalidTokenError_WhenTokenHasExpired()
    {
        // Arrange
        var token = factory.CreateToken(expires: DateTime.UtcNow.AddHours(-1));

        // Act
        var response = await SendWithTokenAsync(token);

        // Assert
        AssertInvalidTokenChallenge(response);
    }

    private async Task<HttpResponseMessage> SendWithTokenAsync(string token)
    {
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/customers/{CustomerIdThatDoesNotExist}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    private static void AssertInvalidTokenChallenge(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var challenge = Assert.Single(response.Headers.WwwAuthenticate);
        Assert.Equal("Bearer", challenge.Scheme);
        Assert.Contains("error=\"invalid_token\"", challenge.Parameter);
    }
}
