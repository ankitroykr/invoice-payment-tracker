using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Infrastructure.Data.Repositories;

public abstract class Repository<T>(InvoiceTrackerDbContext dbContext) : IRepository<T> where T : class, IAggregateRoot
{
    public IUnitOfWork UnitOfWork => dbContext;

    public void Add(T entity)
    {
        dbContext.Set<T>().Add(entity);
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<T>().FindAsync([id], cancellationToken);
    }
}
