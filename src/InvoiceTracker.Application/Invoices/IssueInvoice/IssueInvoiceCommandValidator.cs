using FluentValidation;

namespace InvoiceTracker.Application.Invoices.IssueInvoice;

/// <summary>
/// Validates the input of an <see cref="IssueInvoiceCommand"/> before it is handled.
/// </summary>
public sealed class IssueInvoiceCommandValidator : AbstractValidator<IssueInvoiceCommand>
{
    /// <summary>
    /// Sets up the rules: the invoice identifier must be positive, and the due date must be today or later.
    /// </summary>
    /// <param name="timeProvider">Gives today's date in UTC, the earliest due date allowed.</param>
    public IssueInvoiceCommandValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.InvoiceId).GreaterThan(0);
        RuleFor(x => x.DueDate).GreaterThanOrEqualTo(_ => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime));
    }
}
