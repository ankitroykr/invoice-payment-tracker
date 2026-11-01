namespace InvoiceTracker.Domain.Invoices;

/// <summary>
/// The stage an <see cref="Invoice"/> has reached. An invoice starts as a draft, is issued, then ends as paid or void.
/// </summary>
public enum InvoiceStatus
{
    /// <summary>
    /// The invoice is still being prepared, so its invoice lines can be added.
    /// </summary>
    Draft,

    /// <summary>
    /// The invoice has been sent to the customer and can no longer be edited.
    /// </summary>
    Issued,

    /// <summary>
    /// The full amount of the invoice has been paid.
    /// </summary>
    Paid,

    /// <summary>
    /// The invoice was issued and then cancelled, so nothing is owed on it.
    /// </summary>
    Void
}
