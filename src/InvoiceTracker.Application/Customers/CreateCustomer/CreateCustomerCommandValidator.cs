using FluentValidation;

using InvoiceTracker.Domain.Customers;

namespace InvoiceTracker.Application.Customers.CreateCustomer;

/// <summary>
/// Validates the input of a <see cref="CreateCustomerCommand"/> before it is handled.
/// </summary>
public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Customer.NameMaxLength);
        RuleFor(command => command.Email).NotEmpty().MaximumLength(Customer.EmailMaxLength).EmailAddress();
    }
}
