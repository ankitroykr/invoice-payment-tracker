using InvoiceTracker.Api.Common;
using InvoiceTracker.Api.Security;
using InvoiceTracker.Application.Invoices;
using InvoiceTracker.Application.Invoices.CreateInvoice;
using InvoiceTracker.Application.Invoices.IssueInvoice;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.Invoices;

/// <summary>
/// Creates draft invoices, issues them, and gets them by their identifier.
/// </summary>
/// <param name="createInvoiceHandler">Creates a draft invoice from the request.</param>
/// <param name="invoiceQueries">Reads an invoice for the response.</param>
/// <param name="issueInvoiceHandler">Issues a draft invoice.</param>
public sealed class InvoicesController(
    CreateInvoiceHandler createInvoiceHandler,
    IInvoiceQueries invoiceQueries,
    IssueInvoiceHandler issueInvoiceHandler)
    : ApiController
{
    // No <param> for cancellationToken: .NET 10 shows its text as the request body's description
    // (https://github.com/dotnet/aspnetcore/issues/65805). Put it back once the .NET version this
    // project runs on includes the fix.
    /// <summary>
    /// Creates a draft invoice with its lines.
    /// </summary>
    /// <remarks>
    /// Each line total is rounded to the currency's decimal places, with halves away from zero.
    /// The invoice total is the sum of the rounded lines.
    /// </remarks>
    /// <param name="request">The customer, the currency and at least one line.</param>
    /// <response code="201">The draft invoice was created. The Location header points to it.</response>
    /// <response code="400">The request is invalid. The problems are listed for each field.</response>
    /// <response code="401">There is no access token, or it is invalid or expired.</response>
    /// <response code="403">The token doesn't have the InvoiceTracker.ReadWrite role.</response>
    /// <response code="422">The customer doesn't exist, or the currency isn't supported.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, "application/problem+json")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<ActionResult> CreateInvoice(CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();

        var result = await createInvoiceHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Error!);
        }

        return CreatedAtAction(nameof(GetInvoice), new { id = result.Value }, null);
    }

    // No <param> for cancellationToken, for the same reason as CreateInvoice above
    // (https://github.com/dotnet/aspnetcore/issues/65805).
    /// <summary>
    /// Issues a draft invoice: gives it the next invoice number for this year, dates it today in UTC,
    /// and sets its due date.
    /// </summary>
    /// <param name="id">The identifier of the draft invoice to issue.</param>
    /// <param name="request">The due date, today or later.</param>
    /// <response code="204">The invoice was issued.</response>
    /// <response code="400">The request is invalid. The problems are listed for each field.</response>
    /// <response code="401">There is no access token, or it is invalid or expired.</response>
    /// <response code="403">The token doesn't have the InvoiceTracker.ReadWrite role.</response>
    /// <response code="404">There is no invoice with that identifier.</response>
    /// <response code="422">The invoice isn't a draft, or it has no lines.</response>
    [HttpPost("{id:int}/issue")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, "application/problem+json")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<ActionResult> IssueInvoice(int id, IssueInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(id);

        var result = await issueInvoiceHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Error!);
        }

        return NoContent();
    }

    /// <summary>
    /// Gets an invoice by its identifier, with its lines and total.
    /// </summary>
    /// <param name="id">The invoice's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <response code="200">The invoice with its status, number, dates, total and lines.</response>
    /// <response code="401">There is no access token, or it is invalid or expired.</response>
    /// <response code="403">The token doesn't have the InvoiceTracker.Read or InvoiceTracker.ReadWrite role.</response>
    /// <response code="404">There is no invoice with that identifier.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<InvoiceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [Authorize(Policy = Policies.CanRead)]
    public async Task<ActionResult<InvoiceResponse>> GetInvoice(int id, CancellationToken cancellationToken)
    {
        var invoice = await invoiceQueries.GetInvoiceByIdAsync(id, cancellationToken);

        if (invoice is null)
        {
            return NotFound();
        }

        return Ok(invoice);
    }
}
