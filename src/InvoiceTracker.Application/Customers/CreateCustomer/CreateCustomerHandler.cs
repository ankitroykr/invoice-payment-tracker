using FluentValidation;
using FluentValidation.Results;

using InvoiceTracker.Application.Common;
using InvoiceTracker.Domain.Common;
using InvoiceTracker.Domain.Customers;

namespace InvoiceTracker.Application.Customers.CreateCustomer;

/// <summary>
/// Handles <see cref="CreateCustomerCommand"/> by adding a new customer and saving it.
/// </summary>
/// <param name="validator">Validates the command.</param>
/// <param name="customerRepository">The repository the new customer is added to.</param>
/// <param name="unitOfWork">Saves the new customer to the database.</param>
public sealed class CreateCustomerHandler(
    IValidator<CreateCustomerCommand> validator,
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork)
{
    /// <summary>
    /// Creates a customer from the command and saves it.
    /// </summary>
    /// <param name="command">The name and email of the customer to create.</param>
    /// <param name="cancellationToken">Cancels the save.</param>
    /// <returns>
    /// A successful result holding the new customer's identifier, or a validation error
    /// with field errors when the command is invalid.
    /// </returns>
    public async Task<Result<int>> HandleAsync(CreateCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationResult validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure<int>(validationResult.ToValidationError());
        }

        var customer = Customer.Create(command.Name, command.Email);
        customerRepository.Add(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(customer.Id);
    }
}
