using InvoiceTracker.Application.Customers;

using Microsoft.EntityFrameworkCore;

namespace InvoiceTracker.Infrastructure.Data.Queries;

public sealed class CustomerQueries(InvoiceTrackerDbContext dbContext) : ICustomerQueries
{
    public async Task<CustomerResponse?> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CustomerResponse(c.Id, c.Name, c.Email))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
