namespace InvoiceTracker.Domain.Common;

/// <summary>
/// Marks an entity as an aggregate root: the only entity in its group that is loaded and saved through a repository.
/// </summary>
public interface IAggregateRoot
{
}
