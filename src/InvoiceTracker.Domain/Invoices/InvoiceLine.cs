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
    /// The identifier of the invoice the line belongs to.
    /// </summary>
    public int InvoiceId { get; private set; }

    /// <summary>
    /// What the line is for, for example, the goods or service supplied.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// How many units the line is for.
    /// </summary>
    public decimal Quantity { get; private set; }

    /// <summary>
    /// The price of one unit, in the invoice's currency.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// The quantity multiplied by the unit price, in the invoice's currency, rounded to its minor units with
    /// halves away from zero. It's worked out once when the line is added and then stored.
    /// </summary>
    public decimal Total { get; private set; }

    // EF Core uses this to load lines from the database. The other constructor takes a Money and the
    // currency's minor units, which aren't columns, so EF creates the line empty and sets the properties
    // straight after. The null! only keeps the compiler quiet until then.
    private InvoiceLine()
    {
        Description = null!;
    }

    internal InvoiceLine(string description, decimal quantity, Money unitPrice, int currencyMinorUnits)
    {
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice.Amount;
        Total = new Money(quantity * unitPrice.Amount, unitPrice.Currency).Round(currencyMinorUnits).Amount;
    }
}
