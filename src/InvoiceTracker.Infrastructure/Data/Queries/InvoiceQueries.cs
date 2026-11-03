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
        // Totals are already rounded but come back from decimal(19,4) as 220.0000. Rounding again
        // doesn't change them, it just shows them with the currency's decimal places: 220.00.
        var minorUnits = invoice.CurrencyMinorUnits;

        return new InvoiceResponse(
            invoice.Id,
            invoice.CustomerId,
            invoice.Number?.ToString(),
            invoice.Status.ToString(),
            invoice.Currency,
            invoice.IssueDate,
            invoice.DueDate,
            invoice.Total.Round(minorUnits).Amount,
            [
                .. invoice.Lines.Select(invoiceLine => new InvoiceLineResponse(
                    invoiceLine.Description,
                    invoiceLine.Quantity,
                    invoiceLine.UnitPrice,
                    Math.Round(invoiceLine.Total, minorUnits)))
            ]);
    }
}
