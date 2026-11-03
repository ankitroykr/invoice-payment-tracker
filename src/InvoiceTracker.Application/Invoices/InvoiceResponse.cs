namespace InvoiceTracker.Application.Invoices;

/// <summary>
/// An invoice as returned to API callers.
/// </summary>
/// <param name="Id">The invoice's identifier, assigned by the database.</param>
/// <param name="CustomerId">The identifier of the customer the invoice is for.</param>
/// <param name="Number">The invoice number, for example, INV-2026-0001, or <see langword="null"/> while it is a draft.</param>
/// <param name="Status">The invoice's status: Draft, Issued, Paid or Void.</param>
/// <param name="Currency">The three-letter currency code all amounts on the invoice are in.</param>
/// <param name="IssueDate">The date the invoice was issued, or <see langword="null"/> while it is a draft.</param>
/// <param name="DueDate">The date payment is due, or <see langword="null"/> while it is a draft.</param>
/// <param name="Total">The sum of all line totals.</param>
/// <param name="Lines">The invoice's lines.</param>
public sealed record InvoiceResponse(
    int Id,
    int CustomerId,
    string? Number,
    string Status,
    string Currency,
    DateOnly? IssueDate,
    DateOnly? DueDate,
    decimal Total,
    IReadOnlyList<InvoiceLineResponse> Lines);
