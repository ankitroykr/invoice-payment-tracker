using InvoiceTracker.Application.Invoices.IssueInvoice;

namespace InvoiceTracker.Api.Invoices;

/// <summary>
/// The request body to issue a draft invoice.
/// </summary>
/// <param name="DueDate">The date the customer must pay the invoice by, today or later.</param>
public sealed record IssueInvoiceRequest(DateOnly DueDate)
{
    /// <summary>
    /// Maps the request to the command the Application layer handles.
    /// </summary>
    /// <param name="invoiceId">The identifier of the invoice to issue, taken from the route.</param>
    /// <returns>A command for that invoice with the same due date.</returns>
    public IssueInvoiceCommand ToCommand(int invoiceId) => new(invoiceId, DueDate);
}
