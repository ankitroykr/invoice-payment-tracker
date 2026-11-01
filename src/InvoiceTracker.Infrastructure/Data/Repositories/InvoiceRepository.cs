using InvoiceTracker.Domain.Invoices;

using Microsoft.EntityFrameworkCore;

namespace InvoiceTracker.Infrastructure.Data.Repositories;

public sealed class InvoiceRepository(InvoiceTrackerDbContext dbContext)
    : Repository<Invoice>(dbContext), IInvoiceRepository
{
    private readonly InvoiceTrackerDbContext _dbContext = dbContext;

    public override Task<Invoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Invoices
            .Include(i => i.Lines)
            .SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
    }
}
