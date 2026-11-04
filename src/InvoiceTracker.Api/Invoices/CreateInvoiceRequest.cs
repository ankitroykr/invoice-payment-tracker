using InvoiceTracker.Application.Invoices.CreateInvoice;

namespace InvoiceTracker.Api.Invoices;

/// <summary>
/// The request body to create a new draft invoice for a customer.
/// </summary>
/// <param name="CustomerId">The identifier of the customer the invoice is for.</param>
/// <param name="Currency">The three-letter uppercase ISO 4217 code every amount on the invoice is in, for example, GBP.</param>
/// <param name="Lines">The lines to put on the invoice. Each line's unit price is in the invoice's currency.</param>
public sealed record CreateInvoiceRequest(
    int CustomerId,
    string Currency,
    IReadOnlyList<CreateInvoiceLineRequest> Lines)
{
    /// <summary>
    /// Maps the request and its lines to the command the Application layer handles.
    /// </summary>
    /// <returns>A command with the same customer, currency and lines.</returns>
    public CreateInvoiceCommand ToCommand() => new CreateInvoiceCommand(
        CustomerId,
        Currency,
        [.. Lines.Select(line => new CreateInvoiceLine(line.Description, line.Quantity, line.UnitPrice))]);
}
