using FluentValidation;

using InvoiceTracker.Application.Common;
using InvoiceTracker.Domain.Common;
using InvoiceTracker.Domain.Invoices;

namespace InvoiceTracker.Application.Invoices.VoidInvoice;

/// <summary>
/// Handles <see cref="VoidInvoiceCommand"/> by voiding an issued invoice.
/// </summary>
/// <param name="validator">Validates the command.</param>
/// <param name="invoiceRepository">Loads the invoice to void.</param>
/// <param name="unitOfWork">Saves the voided invoice to the database.</param>
public sealed class VoidInvoiceHandler(
    IValidator<VoidInvoiceCommand> validator,
    IInvoiceRepository invoiceRepository,
    IUnitOfWork unitOfWork)
{
    /// <summary>
    /// Voids the invoice the command names.
    /// </summary>
    /// <param name="command">The invoice to void.</param>
    /// <param name="cancellationToken">Cancels the validation, the lookup, and the save.</param>
    /// <returns>
    /// A successful result, a validation error with field errors when the command is invalid,
    /// <see cref="InvoiceErrors.NotFound"/> when the invoice doesn't exist,
    /// or <see cref="InvoiceErrors.NotIssued"/> when it is still a draft or already void.
    /// </returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled.</exception>
    public async Task<Result> HandleAsync(VoidInvoiceCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ToValidationError());
        }

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);

        if (invoice is null)
        {
            return Result.Failure(InvoiceErrors.NotFound);
        }

        var result = invoice.Void();

        if (!result.IsSuccess)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
