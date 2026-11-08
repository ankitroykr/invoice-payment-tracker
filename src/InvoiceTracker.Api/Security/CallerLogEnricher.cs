using Serilog;

namespace InvoiceTracker.Api.Security;

/// <summary>
/// Adds the caller's identity to the request log, so each logged request shows who made it. Pass
/// <see cref="Enrich"/> to Serilog's request logging as its <c>EnrichDiagnosticContext</c> callback.
/// </summary>
public static class CallerLogEnricher
{
    private const string ObjectIdClaim = "oid";
    private const string AuthorizedPartyClaim = "azp";

    /// <summary>
    /// Sets <c>CallerId</c> from the token's <c>oid</c> claim and <c>ClientAppId</c> from its <c>azp</c> claim.
    /// A missing claim, as on an anonymous request, leaves its property off the log entry.
    /// </summary>
    /// <param name="diagnosticContext">The context whose properties Serilog writes to the request's log entry.</param>
    /// <param name="httpContext">The current request, which holds the caller's claims.</param>
    public static void Enrich(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var callerId = httpContext.User.FindFirst(ObjectIdClaim)?.Value;
        var clientAppId = httpContext.User.FindFirst(AuthorizedPartyClaim)?.Value;

        if (callerId is not null)
        {
            diagnosticContext.Set("CallerId", callerId);
        }

        if (clientAppId is not null)
        {
            diagnosticContext.Set("ClientAppId", clientAppId);
        }
    }
}
