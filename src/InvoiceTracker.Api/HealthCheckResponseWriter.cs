using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InvoiceTracker.Api;

/// <summary>
/// Writes health check results as JSON instead of the default plain text.
/// </summary>
public static class HealthCheckResponseWriter
{
    /// <summary>
    /// Writes the overall status and the name, status and description of each check.
    /// </summary>
    /// <param name="context">The HTTP context of the health check request.</param>
    /// <param name="healthReport">The results of the health checks that ran.</param>
    /// <returns>A task that completes when the response is written.</returns>
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
