namespace InvoiceTracker.Application.Invoices.CreateInvoice;

/// <summary>
/// The request to create a new draft invoice for a customer.
/// </summary>
/// <param name="CustomerId">The identifier of the customer the invoice is for.</param>
/// <param name="Currency">The three-letter uppercase ISO 4217 code every amount on the invoice is in, for example, GBP.</param>
/// <param name="Lines">The lines to put on the invoice. Each line's unit price is in <paramref name="Currency"/>.</param>
public sealed record CreateInvoiceCommand(int CustomerId, string Currency, IReadOnlyList<CreateInvoiceLine> Lines);
