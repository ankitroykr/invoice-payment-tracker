using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InvoiceTracker.Api;

public static class HealthCheckResponseWriter
{
    public static async Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        var response = new
        {
            status = healthReport.Status.ToString(),
            checks = healthReport.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description
            })
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}