using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.Invoices;

/// <summary>
/// The errors returned when an <see cref="Invoice"/> refuses an action because a business rule does not allow it.
/// </summary>
public static class InvoiceErrors
{
    /// <summary>
    /// The invoice is no longer a draft, so it cannot be changed or issued.
    /// </summary>
    public static readonly Error NotDraft =
        Error.BusinessRule("Invoice.NotDraft", "Only a draft invoice can be changed or issued.");

    /// <summary>
    /// The invoice has no lines, so it cannot be issued.
    /// </summary>
    public static readonly Error NoLines =
        Error.BusinessRule("Invoice.NoLines", "An invoice needs at least one line before it can be issued.");

    /// <summary>
    /// The invoice has not been issued, so it cannot be voided.
    /// </summary>
    public static readonly Error NotIssued =
        Error.BusinessRule("Invoice.NotIssued", "Only an issued invoice can be voided.");
}
