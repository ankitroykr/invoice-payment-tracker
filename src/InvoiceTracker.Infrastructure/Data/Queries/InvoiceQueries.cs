using InvoiceTracker.Application.Invoices;
using InvoiceTracker.Domain.Invoices;

using Microsoft.EntityFrameworkCore;

namespace InvoiceTracker.Infrastructure.Data.Queries;

public sealed class InvoiceQueries(InvoiceTrackerDbContext dbContext) : IInvoiceQueries
{
    public async Task<InvoiceResponse?> GetInvoiceByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Loads the invoice and maps it in memory, because the invoice number format and the totals
        // are domain code that SQL can't run.
        var invoice = await dbContext.Invoices
            .AsNoTracking()
            .Include(i => i.Lines)
            .SingleOrDefaultAsync(i => i.Id == id, cancellationToken);

        return invoice is null ? null : MapToResponse(invoice);
    }

    private static InvoiceResponse MapToResponse(Invoice invoice)
    {
        return new InvoiceResponse(
            invoice.Id,
            invoice.CustomerId,
            invoice.Number?.ToString(),
            invoice.Status.ToString(),
            invoice.Currency,
            invoice.IssueDate,
            invoice.DueDate,
            invoice.Total.Amount,
            [
                .. invoice.Lines.Select(invoiceLine => new InvoiceLineResponse(
                    invoiceLine.Description,
                    invoiceLine.Quantity,
                    invoiceLine.UnitPrice.Amount,
                    invoiceLine.Total.Amount))
            ]);
    }
}
