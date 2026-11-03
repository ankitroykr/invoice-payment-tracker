namespace InvoiceTracker.Application.Invoices;

/// <summary>
/// Reads invoices for display.
/// </summary>
public interface IInvoiceQueries
{
    /// <summary>
    /// Gets an invoice and its lines by the invoice's identifier.
    /// </summary>
    /// <param name="id">The invoice's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the query.</param>
    /// <returns>The invoice with the given identifier,
    /// or <see langword="null"/> if there is no invoice with that identifier.</returns>
    Task<InvoiceResponse?> GetInvoiceByIdAsync(int id, CancellationToken cancellationToken = default);
}
