using FluentValidation;

using InvoiceTracker.Domain.Invoices;

namespace InvoiceTracker.Application.Invoices.CreateInvoice;

/// <summary>
/// Validates one <see cref="CreateInvoiceLine"/> of a <see cref="CreateInvoiceCommand"/> before it is handled.
/// </summary>
public sealed class CreateInvoiceLineValidator : AbstractValidator<CreateInvoiceLine>
{
    public CreateInvoiceLineValidator()
    {
        RuleFor(invoiceLine => invoiceLine.Description).NotEmpty().MaximumLength(InvoiceLine.DescriptionMaxLength);
        RuleFor(invoiceLine => invoiceLine.Quantity).GreaterThan(0).PrecisionScale(19, 4, true);
        RuleFor(invoiceLine => invoiceLine.UnitPrice).GreaterThanOrEqualTo(0).PrecisionScale(19, 4, true);
    }
}
