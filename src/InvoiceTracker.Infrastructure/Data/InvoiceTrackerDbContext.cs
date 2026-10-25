using InvoiceTracker.Domain.Customers;

using Microsoft.EntityFrameworkCore;

namespace InvoiceTracker.Infrastructure.Data;

public class InvoiceTrackerDbContext(DbContextOptions<InvoiceTrackerDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoiceTrackerDbContext).Assembly);
    }
}
