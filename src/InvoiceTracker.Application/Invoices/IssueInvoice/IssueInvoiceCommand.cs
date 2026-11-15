namespace InvoiceTracker.Application.Invoices.IssueInvoice;

/// <summary>
/// The request to issue a draft invoice, which gives it its invoice number.
/// </summary>
/// <param name="InvoiceId">The identifier of the draft invoice to issue.</param>
/// <param name="DueDate">The date the customer must pay the invoice by.</param>
public sealed record IssueInvoiceCommand(int InvoiceId, DateOnly DueDate);
