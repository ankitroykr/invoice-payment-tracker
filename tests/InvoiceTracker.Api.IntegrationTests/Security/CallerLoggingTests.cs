using System.Net.Http.Headers;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Api.Security;

using Serilog.Events;

namespace InvoiceTracker.Api.IntegrationTests.Security;

public sealed class CallerLoggingTests(InvoiceTrackerApiFactory factory)
{
    private const int IdThatDoesNotExist = int.MaxValue;

    private static readonly TimeSpan LogWaitTimeout = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task Request_LogsCallerAndClientApp_WhenTokenIsValid()
    {
        // Arrange
        var oid = Guid.NewGuid().ToString(); // CallerId - ObjectId
        var azp = Guid.NewGuid().ToString(); // ClientAppId - Authorized Party
        var token = factory.CreateToken(roles: [AppRoles.Read], oid: oid, azp: azp);
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/customers/{IdThatDoesNotExist}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        await client.SendAsync(request, TestContext.Current.CancellationToken);
        var logEvent = await WaitForLogEventAsync(oid);

        // Assert
        Assert.NotNull(logEvent);
        Assert.Equal(azp, ((ScalarValue)logEvent.Properties["ClientAppId"]).Value);
    }

    private async Task<LogEvent?> WaitForLogEventAsync(string callerId)
    {
        var giveUpAt = DateTime.UtcNow + LogWaitTimeout;

        while (DateTime.UtcNow < giveUpAt)
        {
            var logEvent = factory.LogSink.LogEvents.FirstOrDefault(entry => HasCallerId(entry, callerId));

            if (logEvent is not null)
            {
                return logEvent;
            }

            await Task.Delay(50, TestContext.Current.CancellationToken);
        }

        return null;
    }

    private static bool HasCallerId(LogEvent logEvent, string callerId)
    {
        if (!logEvent.Properties.TryGetValue("CallerId", out var value))
        {
            return false;
        }

        if (value is not ScalarValue { Value: string id })
        {
            return false;
        }

        return id == callerId;
    }
}
