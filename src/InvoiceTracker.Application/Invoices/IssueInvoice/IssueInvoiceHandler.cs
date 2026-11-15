using FluentValidation;

using InvoiceTracker.Application.Common;
using InvoiceTracker.Domain.Common;
using InvoiceTracker.Domain.Invoices;

namespace InvoiceTracker.Application.Invoices.IssueInvoice;

/// <summary>
/// Handles <see cref="IssueInvoiceCommand"/> by issuing a draft invoice with the next invoice number for the year.
/// </summary>
/// <param name="validator">Validates the command.</param>
/// <param name="invoiceRepository">Loads the invoice to issue.</param>
/// <param name="invoiceNumberCounter">Hands out the invoice's sequence.</param>
/// <param name="unitOfWork">Saves the issued invoice to the database.</param>
/// <param name="timeProvider">Gives today's date in UTC, which is the issue date and picks the year of the number.</param>
public sealed class IssueInvoiceHandler(
    IValidator<IssueInvoiceCommand> validator,
    IInvoiceRepository invoiceRepository,
    IInvoiceNumberCounter invoiceNumberCounter,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    /// <summary>
    /// Issues the invoice the command names, dated today in UTC, with the next number for this year.
    /// </summary>
    /// <param name="command">The invoice to issue and its due date.</param>
    /// <param name="cancellationToken">Cancels the validation, the lookup, taking the sequence, and the save.</param>
    /// <returns>
    /// A successful result, a validation error with field errors when the command is invalid,
    /// <see cref="InvoiceErrors.NotFound"/> when the invoice doesn't exist,
    /// <see cref="InvoiceErrors.NotDraft"/> when it is no longer a draft,
    /// or <see cref="InvoiceErrors.NoLines"/> when it has no lines.
    /// </returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled.</exception>
    public async Task<Result> HandleAsync(IssueInvoiceCommand command, CancellationToken cancellationToken = default)
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

        var canIssueResult = invoice.CanIssue();

        if (!canIssueResult.IsSuccess)
        {
            return canIssueResult;
        }

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        var sequence = await invoiceNumberCounter.GetNextSequenceAsync(today.Year, cancellationToken);

        var result = invoice.Issue(sequence, today, command.DueDate);

        if (!result.IsSuccess)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
