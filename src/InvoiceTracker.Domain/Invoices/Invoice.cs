using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.Invoices;

/// <summary>
/// A bill to a customer made up of invoice lines, all in one currency. It starts as a draft with no lines.
/// </summary>
public sealed class Invoice : IAggregateRoot
{
    private readonly List<InvoiceLine> _lines = [];

    /// <summary>
    /// The invoice's identifier. The database assigns it when the invoice is saved, so it is 0 on a new invoice.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// The identifier of the customer the invoice is for.
    /// </summary>
    public int CustomerId { get; private set; }

    /// <summary>
    /// The three-letter uppercase ISO 4217 code every amount on the invoice is in, for example, GBP.
    /// </summary>
    public string Currency { get; private set; }

    /// <summary>
    /// The number of decimal places the currency uses, copied from the currency when the invoice is created,
    /// so later changes to the currency don't change this invoice.
    /// </summary>
    public int CurrencyMinorUnits { get; private set; }

    /// <summary>
    /// The stage the invoice has reached. A new invoice is a <see cref="InvoiceStatus.Draft"/>.
    /// </summary>
    public InvoiceStatus Status { get; private set; }

    /// <summary>
    /// The invoice's number, which is null until the invoice is issued.
    /// </summary>
    public InvoiceNumber? Number { get; private set; }

    /// <summary>
    /// The date the invoice was issued, which is null until the invoice is issued.
    /// </summary>
    public DateOnly? IssueDate { get; private set; }

    /// <summary>
    /// The date payment is due, which is null until the invoice is issued.
    /// </summary>
    public DateOnly? DueDate { get; private set; }

    /// <summary>
    /// The invoice's lines. Callers can read them but not add or remove them directly.
    /// </summary>
    public IReadOnlyCollection<InvoiceLine> Lines => _lines.AsReadOnly();

    /// <summary>
    /// The sum of the line totals, in the invoice's currency. It is 0 when the invoice has no lines.
    /// </summary>
    public Money Total => new(_lines.Sum(invoiceLine => invoiceLine.Total), Currency);

    private Invoice(int customerId, string currency, int currencyMinorUnits)
    {
        CustomerId = customerId;
        Currency = currency;
        CurrencyMinorUnits = currencyMinorUnits;
        Status = InvoiceStatus.Draft;
    }

    /// <summary>
    /// Creates a draft invoice with no lines and checks the customer identifier and currency are valid.
    /// </summary>
    /// <param name="customerId">The identifier of the customer the invoice is for, 1 or more.</param>
    /// <param name="currency">A three-letter uppercase ISO 4217 code, for example, GBP.</param>
    /// <param name="currencyMinorUnits">The number of decimal places the currency uses, from 0 to 4.</param>
    /// <returns>A new draft <see cref="Invoice"/> for the given customer and currency.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The customer identifier is 0 or less, or the minor units are below 0 or above 4.
    /// </exception>
    /// <exception cref="ArgumentNullException">The currency is null.</exception>
    /// <exception cref="ArgumentException">
    /// The currency is empty, whitespace, or not three uppercase letters.
    /// </exception>
    public static Invoice Create(int customerId, string currency, int currencyMinorUnits)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(customerId);
        Money.ThrowIfInvalidCode(currency);
        ArgumentOutOfRangeException.ThrowIfNegative(currencyMinorUnits);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(currencyMinorUnits, Money.MaxMinorUnits);

        return new Invoice(customerId, currency, currencyMinorUnits);
    }

    /// <summary>
    /// Adds a line to the invoice while it is still a draft.
    /// </summary>
    /// <param name="description">What the line is for, for example, the goods or service supplied.</param>
    /// <param name="quantity">How many units the line is for, more than 0.</param>
    /// <param name="unitPrice">The price of one unit, 0 or more, in the invoice's currency.</param>
    /// <returns>
    /// A success, or a failure with <see cref="InvoiceErrors.NotDraft"/> when the invoice is no longer a draft.
    /// </returns>
    /// <exception cref="ArgumentNullException">The description or unit price is null.</exception>
    /// <exception cref="ArgumentException">
    /// The description is empty or whitespace, or the unit price is in a different currency from the invoice.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The quantity is 0 or less, or the unit price is negative.
    /// </exception>
    public Result AddLine(string description, decimal quantity, Money unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentNullException.ThrowIfNull(unitPrice);
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice.Amount);

        if (unitPrice.Currency != Currency)
        {
            throw new ArgumentException(
                $"The unit price currency {unitPrice.Currency} must match invoice currency {Currency}.",
                nameof(unitPrice));
        }

        if (Status != InvoiceStatus.Draft)
        {
            return Result.Failure(InvoiceErrors.NotDraft);
        }

        _lines.Add(new InvoiceLine(description, quantity, unitPrice, CurrencyMinorUnits));

        return Result.Success();
    }

    /// <summary>
    /// Issues a draft invoice: gives it a number and its issue and due dates, and moves it to
    /// <see cref="InvoiceStatus.Issued"/>.
    /// </summary>
    /// <param name="sequence">The invoice's place in the issue year's counter, 1 or more.</param>
    /// <param name="issueDate">The date the invoice is issued. Its year becomes part of the invoice number.</param>
    /// <param name="dueDate">The date payment is due, on or after the issue date.</param>
    /// <returns>
    /// A success, or a failure with <see cref="InvoiceErrors.NotDraft"/> when the invoice is no longer a draft,
    /// or <see cref="InvoiceErrors.NoLines"/> when it has no lines.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The due date is before the issue date, or the sequence is less than 1.
    /// </exception>
    public Result Issue(int sequence, DateOnly issueDate, DateOnly dueDate)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(dueDate, issueDate);

        if (Status != InvoiceStatus.Draft)
        {
            return Result.Failure(InvoiceErrors.NotDraft);
        }

        if (_lines.Count == 0)
        {
            return Result.Failure(InvoiceErrors.NoLines);
        }

        Number = new InvoiceNumber(issueDate.Year, sequence);
        IssueDate = issueDate;
        DueDate = dueDate;
        Status = InvoiceStatus.Issued;

        return Result.Success();
    }

    /// <summary>
    /// Cancels an issued invoice and moves it to <see cref="InvoiceStatus.Void"/>, so nothing is owed on it.
    /// </summary>
    /// <returns>
    /// A success, or a failure with <see cref="InvoiceErrors.NotIssued"/> when the invoice has not been issued.
    /// </returns>
    public Result Void()
    {
        if (Status != InvoiceStatus.Issued)
        {
            return Result.Failure(InvoiceErrors.NotIssued);
        }

        Status = InvoiceStatus.Void;

        return Result.Success();
    }
}
