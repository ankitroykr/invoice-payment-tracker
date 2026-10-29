using InvoiceTracker.Application.Customers.CreateCustomer;

namespace InvoiceTracker.Api.Customers;

public sealed record CreateCustomerRequest(string Name, string Email)
{
    public CreateCustomerCommand ToCommand() => new CreateCustomerCommand(Name, Email);
}
