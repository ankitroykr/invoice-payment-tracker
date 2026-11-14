namespace InvoiceTracker.Application.Invoices;

/// <summary>
/// Hands out invoice numbers from a counter that restarts each year.
/// </summary>
public interface IInvoiceNumberCounter
{
    /// <summary>
    /// Increases the counter for the given year and returns the new value.
    /// </summary>
    /// <param name="year">The year the invoice is issued in.</param>
    /// <param name="cancellationToken">A token to cancel the update.</param>
    /// <returns>The next number for that year, starting at 1.</returns>
    Task<int> GetNextAsync(int year, CancellationToken cancellationToken = default);
}
