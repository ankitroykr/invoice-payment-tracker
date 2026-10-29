namespace InvoiceTracker.Application.Customers;

/// <summary>
/// Reads customers for display, without loading them as domain entities.
/// </summary>
public interface ICustomerQueries
{
    /// <summary>
    /// Gets a customer by their identifier.
    /// </summary>
    /// <param name="id">The customer's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the query.</param>
    /// <returns>The customer with the given identifier,
    /// or <see langword="null"/> if there is no customer with that identifier.</returns>
    Task<CustomerResponse?> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default);
}
