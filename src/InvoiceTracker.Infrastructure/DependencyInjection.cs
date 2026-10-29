using InvoiceTracker.Domain.Common;
using InvoiceTracker.Domain.Customers;
using InvoiceTracker.Infrastructure.Data;
using InvoiceTracker.Infrastructure.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceTracker.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the database and repositories
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <param name="connectionString">The SQL Server connection string.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<InvoiceTrackerDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<InvoiceTrackerDbContext>());

        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }
}
