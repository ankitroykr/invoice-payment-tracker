using Microsoft.EntityFrameworkCore;

namespace InvoiceTracker.Infrastructure.Data;

public class InvoiceTrackerDbContext(DbContextOptions<InvoiceTrackerDbContext> options) : DbContext(options)
{
}
