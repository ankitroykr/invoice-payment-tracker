namespace InvoiceTracker.Application.Invoices;

/// <summary>
/// An invoice line as returned to API callers.
/// </summary>
/// <param name="Description">What the line is for.</param>
/// <param name="Quantity">How many units the line is for.</param>
/// <param name="UnitPrice">The price of one unit, in the invoice's currency.</param>
/// <param name="Total">The quantity times the unit price.</param>
public sealed record InvoiceLineResponse(
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal Total);
