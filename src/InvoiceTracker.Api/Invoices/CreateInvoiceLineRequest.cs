namespace InvoiceTracker.Api.Invoices;

public sealed record CreateInvoiceLineRequest(string Description, decimal Quantity, decimal UnitPrice);
