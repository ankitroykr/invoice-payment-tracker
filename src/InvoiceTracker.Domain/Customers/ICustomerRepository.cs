using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.Customers;

/// <summary>
/// Loads and adds customers.
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
}
