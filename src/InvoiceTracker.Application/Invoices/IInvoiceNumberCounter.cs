namespace InvoiceTracker.Application.Invoices;

/// <summary>
/// Hands out the sequence part of invoice numbers from a counter that restarts each year.
/// </summary>
public interface IInvoiceNumberCounter
{
    /// <summary>
    /// Increases the counter for the given year and returns the new sequence.
    /// </summary>
    /// <param name="year">The year the invoice is issued in.</param>
    /// <param name="cancellationToken">A token to cancel the update.</param>
    /// <returns>The next sequence for that year, starting at 1.</returns>
    Task<int> GetNextSequenceAsync(int year, CancellationToken cancellationToken = default);
}
