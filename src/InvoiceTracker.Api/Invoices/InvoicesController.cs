using InvoiceTracker.Api.Common;
using InvoiceTracker.Application.Invoices;
using InvoiceTracker.Application.Invoices.CreateInvoice;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.Invoices;

public sealed class InvoicesController(CreateInvoiceHandler createInvoiceHandler, IInvoiceQueries invoiceQueries)
    : ApiController
{
    [HttpPost]
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

    [HttpGet("{id:int}")]
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
