using InvoiceTracker.Domain.Customers;

namespace InvoiceTracker.Infrastructure.Data.Repositories;

public sealed class CustomerRepository(InvoiceTrackerDbContext dbContext)
    : Repository<Customer>(dbContext), ICustomerRepository
{
}
