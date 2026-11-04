namespace InvoiceTracker.Api.Invoices;

/// <summary>
/// One line of the request body to create a new invoice: what was supplied, how many, and the price of each.
/// </summary>
/// <param name="Description">What the line is for, for example, the goods or service supplied.</param>
/// <param name="Quantity">How many units the line is for.</param>
/// <param name="UnitPrice">The price of one unit, in the invoice's currency.</param>
public sealed record CreateInvoiceLineRequest(string Description, decimal Quantity, decimal UnitPrice);
