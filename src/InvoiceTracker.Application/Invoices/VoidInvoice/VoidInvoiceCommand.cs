namespace InvoiceTracker.Application.Invoices.VoidInvoice;

/// <summary>
/// The request to void an issued invoice, so nothing is owed on it.
/// </summary>
/// <param name="InvoiceId">The identifier of the issued invoice to void.</param>
public sealed record VoidInvoiceCommand(int InvoiceId);
