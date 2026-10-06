using Microsoft.EntityFrameworkCore;

namespace InvoiceTracker.Api.Data;

public class InvoiceTrackerDbContext(DbContextOptions<InvoiceTrackerDbContext> options) : DbContext(options)
{
}
