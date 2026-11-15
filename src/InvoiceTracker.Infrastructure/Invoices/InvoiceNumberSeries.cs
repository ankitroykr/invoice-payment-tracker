namespace InvoiceTracker.Infrastructure.Invoices;

public sealed class InvoiceNumberSeries
{
    public int LastSequence { get; private set; }

    public int Year { get; private set; }
}
