using InvoiceTracker.Application.Invoices;
using InvoiceTracker.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace InvoiceTracker.Infrastructure.Invoices;

public sealed class InvoiceNumberCounter(InvoiceTrackerDbContext dbContext) : IInvoiceNumberCounter
{
    public async Task<int> GetNextAsync(int year, CancellationToken cancellationToken = default)
    {
        var numbers = await dbContext.Database.SqlQuery<int>($"""
                      INSERT INTO dbo.InvoiceNumberSeries (Year, LastNumber)
                      SELECT {year}, 0
                      WHERE NOT EXISTS (
                          SELECT 1
                          FROM dbo.InvoiceNumberSeries WITH (UPDLOCK, HOLDLOCK)
                          WHERE Year = {year});

                      UPDATE dbo.InvoiceNumberSeries
                      SET LastNumber = LastNumber + 1
                      OUTPUT inserted.LastNumber AS Value
                      WHERE Year = {year};
                      """).ToListAsync(cancellationToken);

        return numbers.Single();
    }
}
