using FluentValidation;

using InvoiceTracker.Application.Customers.CreateCustomer;

using Microsoft.Extensions.DependencyInjection;

namespace InvoiceTracker.Application;

/// <summary>
/// Registers the Application layer's services with the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the validators and command handlers, all with a scoped lifetime.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<CreateCustomerHandler>();

        return services;
    }
}
