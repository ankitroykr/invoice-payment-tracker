using FluentValidation;

using InvoiceTracker.Application.Common;
using InvoiceTracker.Application.Currencies;
using InvoiceTracker.Domain.Common;
using InvoiceTracker.Domain.Customers;
using InvoiceTracker.Domain.Invoices;

namespace InvoiceTracker.Application.Invoices.CreateInvoice;

/// <summary>
/// Handles <see cref="CreateInvoiceCommand"/> by creating a draft invoice with its lines and saving it.
/// </summary>
/// <param name="validator">Validates the command.</param>
/// <param name="invoiceRepository">The repository the new invoice is added to.</param>
/// <param name="customerRepository">Checks the customer the invoice is for exists.</param>
/// <param name="currencyProvider">Checks the invoice's currency is supported.</param>
/// <param name="unitOfWork">Saves the new invoice to the database.</param>
public sealed class CreateInvoiceHandler(
    IValidator<CreateInvoiceCommand> validator,
    IInvoiceRepository invoiceRepository,
    ICustomerRepository customerRepository,
    ICurrencyProvider currencyProvider,
    IUnitOfWork unitOfWork)
{
    /// <summary>
    /// Creates a draft invoice from the command, adds its lines, and saves it.
    /// </summary>
    /// <param name="command">The customer, currency, and lines of the invoice to create.</param>
    /// <param name="cancellationToken">Cancels the lookups and the save.</param>
    /// <returns>
    /// A successful result holding the new invoice's identifier, a validation error with field errors
    /// when the command is invalid, <see cref="InvoiceErrors.CurrencyNotSupported"/> when the currency isn't supported,
    /// or <see cref="InvoiceErrors.CustomerNotFound"/> when the customer doesn't exist.
    /// </returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled.</exception>
    public async Task<Result<int>> HandleAsync(CreateInvoiceCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure<int>(validationResult.ToValidationError());
        }

        var currency = await currencyProvider.FindAsync(command.Currency, cancellationToken);

        if (currency is null)
        {
            return Result.Failure<int>(InvoiceErrors.CurrencyNotSupported);
        }

        var customer = await customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure<int>(InvoiceErrors.CustomerNotFound);
        }

        var invoice = Invoice.Create(command.CustomerId, command.Currency);

        foreach (var line in command.Lines)
        {
            var addLineResult = invoice.AddLine(line.Description, line.Quantity,
                new Money(line.UnitPrice, command.Currency));

            if (addLineResult.IsFailure)
            {
                return Result.Failure<int>(addLineResult.Error!);
            }
        }

        invoiceRepository.Add(invoice);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(invoice.Id);
    }
}
