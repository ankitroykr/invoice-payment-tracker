using InvoiceTracker.Application.Invoices;
using InvoiceTracker.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace InvoiceTracker.Infrastructure.Invoices;

public sealed class InvoiceNumberCounter(InvoiceTrackerDbContext dbContext) : IInvoiceNumberCounter
{
    public async Task<int> GetNextSequenceAsync(int year, CancellationToken cancellationToken = default)
    {
        var sequences = await dbContext.Database.SqlQuery<int>($"""
                      INSERT INTO dbo.InvoiceNumberSeries (Year, LastSequence)
                      SELECT {year}, 0
                      WHERE NOT EXISTS (
                          SELECT 1
                          FROM dbo.InvoiceNumberSeries WITH (UPDLOCK, HOLDLOCK)
                          WHERE Year = {year});

                      UPDATE dbo.InvoiceNumberSeries
                      SET LastSequence = LastSequence + 1
                      OUTPUT inserted.LastSequence AS Value
                      WHERE Year = {year};
                      """).ToListAsync(cancellationToken);

        return sequences.Single();
    }
}
