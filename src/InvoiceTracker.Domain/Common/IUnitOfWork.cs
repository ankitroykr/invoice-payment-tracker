namespace InvoiceTracker.Domain.Common;

/// <summary>
/// Saves all changes made through the repositories in one transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all pending changes to the database in one transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the save.</param>
    /// <returns>The number of rows written to the database.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled.</exception>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
