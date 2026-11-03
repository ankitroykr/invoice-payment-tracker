using FluentValidation;

using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Application.Invoices.CreateInvoice;

/// <summary>
/// Validates the input of a <see cref="CreateInvoiceCommand"/> before it is handled.
/// Each line is checked by <see cref="CreateInvoiceLineValidator"/>.
/// </summary>
public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(command => command.CustomerId).GreaterThan(0);
        RuleFor(command => command.Currency)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(Money.IsValidCode)
            .WithMessage("'{PropertyName}' must be a three-letter uppercase ISO 4217 code, for example, GBP.");
        RuleFor(command => command.Lines).NotEmpty();
        RuleForEach(command => command.Lines).SetValidator(new CreateInvoiceLineValidator());
    }
}
