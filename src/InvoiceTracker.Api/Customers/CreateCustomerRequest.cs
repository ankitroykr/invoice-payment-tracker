using InvoiceTracker.Application.Customers.CreateCustomer;

namespace InvoiceTracker.Api.Customers;

/// <summary>
/// The request body to create a new customer.
/// </summary>
/// <param name="Name">The customer's name.</param>
/// <param name="Email">The customer's email address.</param>
public sealed record CreateCustomerRequest(string Name, string Email)
{
    /// <summary>
    /// Maps the request to the command the Application layer handles.
    /// </summary>
    /// <returns>A command with the same name and email address.</returns>
    public CreateCustomerCommand ToCommand() => new CreateCustomerCommand(Name, Email);
}
