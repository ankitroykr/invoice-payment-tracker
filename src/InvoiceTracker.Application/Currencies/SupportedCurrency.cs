namespace InvoiceTracker.Application.Currencies;

/// <summary>
/// A currency invoices can use.
/// </summary>
/// <param name="Code">The ISO 4217 code, for example, GBP.</param>
/// <param name="MinorUnits">The number of decimal places totals are rounded to, for example, 2 for GBP.</param>
/// <param name="Name">The currency's name, for example, Pound Sterling.</param>
public sealed record SupportedCurrency(string Code, int MinorUnits, string Name);
