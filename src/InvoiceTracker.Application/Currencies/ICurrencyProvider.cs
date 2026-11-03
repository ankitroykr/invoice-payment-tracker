namespace InvoiceTracker.Application.Currencies;

/// <summary>
/// Looks up the currencies invoices can use.
/// </summary>
public interface ICurrencyProvider
{
    /// <summary>
    /// Finds a supported currency by its ISO 4217 code.
    /// </summary>
    /// <param name="code">The three-letter code, for example, GBP.</param>
    /// <param name="cancellationToken">A token to cancel the lookup.</param>
    /// <returns>The currency with the given code,
    /// or <see langword="null"/> if it isn't supported.</returns>
    Task<SupportedCurrency?> FindAsync(string code, CancellationToken cancellationToken = default);
}
