namespace InvoiceTracker.Domain.Invoices;

/// <summary>
/// An invoice's number, made of the year and a counter that restarts each year, for example, INV-2026-0001.
/// It can't be changed once created.
/// </summary>
public sealed record InvoiceNumber
{
    /// <summary>
    /// The year the invoice was issued in.
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// The invoice's place in the year's counter, starting at 1.
    /// </summary>
    public int Sequence { get; }

    /// <summary>
    /// Creates an invoice number and checks the year and sequence are in range.
    /// </summary>
    /// <param name="year">The year the invoice was issued in, from 1 to 9999.</param>
    /// <param name="sequence">The invoice's place in the year's counter, 1 or more. There's no upper limit.</param>
    /// <exception cref="ArgumentOutOfRangeException">The year is outside 1 to 9999, or the sequence is less than 1.</exception>
    public InvoiceNumber(int year, int sequence)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(year, DateOnly.MinValue.Year);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(year, DateOnly.MaxValue.Year);
        ArgumentOutOfRangeException.ThrowIfLessThan(sequence, 1);

        Year = year;
        Sequence = sequence;
    }

    /// <summary>
    /// Formats the number as INV-year-sequence, with the sequence padded to at least 4 digits.
    /// </summary>
    /// <returns>The invoice number, for example, INV-2026-0001 or INV-2026-10000.</returns>
    public override string ToString()
    {
        return $"INV-{Year}-{Sequence:D4}";
    }
}
