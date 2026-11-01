using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.Invoices;

/// <summary>
/// One item on an <see cref="Invoice"/>: what was supplied, how many, and the price of each.
/// </summary>
public sealed class InvoiceLine
{
    /// <summary>
    /// The maximum allowed length for a line's description.
    /// </summary>
    public const int DescriptionMaxLength = 500;

    /// <summary>
    /// The line's identifier. The database assigns it when the line is saved, so it is 0 on a new line.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// What the line is for, for example, the goods or service supplied.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// How many units the line is for.
    /// </summary>
    public decimal Quantity { get; private set; }

    /// <summary>
    /// The price of one unit.
    /// </summary>
    public Money UnitPrice { get; private set; }

    /// <summary>
    /// The quantity multiplied by the unit price, in the unit price's currency.
    /// </summary>
    public Money Total => new(Quantity * UnitPrice.Amount, UnitPrice.Currency);

    internal InvoiceLine(string description, decimal quantity, Money unitPrice)
    {
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
