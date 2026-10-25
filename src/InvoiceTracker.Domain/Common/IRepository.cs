namespace InvoiceTracker.Domain.Common;

/// <summary>
/// Loads and adds aggregate roots of one type. Changes are saved through <see cref="UnitOfWork"/>.
/// </summary>
/// <typeparam name="T">The aggregate root the repository stores.</typeparam>
public interface IRepository<T> where T : class, IAggregateRoot
{
    /// <summary>
    /// Gets the unit of work that saves the changes made through this repository.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// Adds a new entity. It is written to the database on the next <see cref="IUnitOfWork.SaveChangesAsync"/>.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(T entity);

    /// <summary>
    /// Gets the entity with the given identifier.
    /// </summary>
    /// <param name="id">The entity's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the query.</param>
    /// <returns>The entity, or null if none has that identifier.</returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled.</exception>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
