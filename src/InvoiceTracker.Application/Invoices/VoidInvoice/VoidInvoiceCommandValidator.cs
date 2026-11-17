using FluentValidation;

namespace InvoiceTracker.Application.Invoices.VoidInvoice;

/// <summary>
/// Validates the input of a <see cref="VoidInvoiceCommand"/> before it is handled.
/// </summary>
public sealed class VoidInvoiceCommandValidator : AbstractValidator<VoidInvoiceCommand>
{
    /// <summary>
    /// Sets up the rule: the invoice identifier must be positive.
    /// </summary>
    public VoidInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).GreaterThan(0);
    }
}
