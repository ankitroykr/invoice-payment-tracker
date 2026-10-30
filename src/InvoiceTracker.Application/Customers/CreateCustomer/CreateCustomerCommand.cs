namespace InvoiceTracker.Application.Customers.CreateCustomer;

/// <summary>
/// The request to create a new customer.
/// </summary>
/// <param name="Name">The customer's name.</param>
/// <param name="Email">The customer's email address.</param>
public sealed record CreateCustomerCommand(string Name, string Email);
