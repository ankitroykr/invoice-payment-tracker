using InvoiceTracker.Application.Invoices.CreateInvoice;

namespace InvoiceTracker.Api.Invoices;

public sealed record CreateInvoiceRequest(
    int CustomerId,
    string Currency,
    IReadOnlyList<CreateInvoiceLineRequest> Lines)
{
    public CreateInvoiceCommand ToCommand() => new CreateInvoiceCommand(
        CustomerId,
        Currency,
        [.. Lines.Select(line => new CreateInvoiceLine(line.Description, line.Quantity, line.UnitPrice))]);
}
