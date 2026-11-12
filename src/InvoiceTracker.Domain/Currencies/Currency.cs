using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.Currencies;

/// <summary>
/// A currency that invoices can be raised in. Create one with <see cref="Create"/>.
/// </summary>
public sealed class Currency
{
    /// <summary>
    /// The maximum allowed length for a currency's name.
    /// </summary>
    public const int NameMaxLength = 100;

    /// <summary>
    /// The three-letter uppercase ISO 4217 code, for example, GBP.
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// The number of decimal places ISO 4217 sets for the currency, for example, 2 for GBP.
    /// </summary>
    public int MinorUnits { get; private set; }

    /// <summary>
    /// The currency's ISO 4217 name, for example, Pound Sterling.
    /// </summary>
    public string Name { get; private set; }

    private Currency(string code, int minorUnits, string name)
    {
        Code = code;
        MinorUnits = minorUnits;
        Name = name;
    }

    /// <summary>
    /// Creates a currency and checks its code, minor units and name are valid.
    /// </summary>
    /// <param name="code">A three-letter uppercase ISO 4217 code, for example, GBP.</param>
    /// <param name="minorUnits">The number of decimal places ISO 4217 sets for the currency, from 0 to 4.</param>
    /// <param name="name">The currency's ISO 4217 name, for example, Pound Sterling.</param>
    /// <returns>A new <see cref="Currency"/>.</returns>
    /// <exception cref="ArgumentNullException">The code or name is null.</exception>
    /// <exception cref="ArgumentException">
    /// The code is not three uppercase letters, or the name is empty, whitespace or too long.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">The minor units are below 0 or above 4.</exception>
    public static Currency Create(string code, int minorUnits, string name)
    {
        Money.ThrowIfInvalidCode(code);
        ArgumentOutOfRangeException.ThrowIfNegative(minorUnits);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minorUnits, Money.MaxMinorUnits);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return name.Length > NameMaxLength
            ? throw new ArgumentException($"Currency name must be {NameMaxLength} characters or fewer.", nameof(name))
            : new Currency(code, minorUnits, name);
    }
}
