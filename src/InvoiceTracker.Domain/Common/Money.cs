using System.Globalization;

namespace InvoiceTracker.Domain.Common;

/// <summary>
/// An amount of money in a specific currency. It can't be changed once created, and the amount can be negative.
/// </summary>
public sealed record Money
{
    /// <summary>
    /// The amount, which can be negative.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// The ISO 4217 currency code, for example, GBP.
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Creates an amount in the given currency and checks the currency code is valid.
    /// </summary>
    /// <param name="amount">The amount, which can be negative.</param>
    /// <param name="currency">A three-letter uppercase ISO 4217 code, for example, GBP.</param>
    /// <exception cref="ArgumentNullException">The currency is null.</exception>
    /// <exception cref="ArgumentException">The currency is empty, whitespace, or not three uppercase letters.</exception>
    public Money(decimal amount, string currency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);

        if (currency.Length != 3 || !currency.All(char.IsAsciiLetterUpper))
        {
            throw new ArgumentException(
                $"Currency '{currency}' is not a valid ISO 4217 code; expected 3 uppercase letters such as GBP.",
                nameof(currency));
        }

        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Creates a zero amount, the starting point for adding up totals.
    /// </summary>
    /// <param name="currency">A three-letter uppercase ISO 4217 code, for example, GBP.</param>
    /// <returns>A <see cref="Money"/> of 0 in the given currency.</returns>
    public static Money Zero(string currency) => new(0, currency);

    public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Amount} {Currency}");

    /// <summary>
    /// Adds two amounts in the same currency.
    /// </summary>
    /// <param name="left">The first amount.</param>
    /// <param name="right">The amount to add.</param>
    /// <returns>A new <see cref="Money"/> with the total.</returns>
    /// <exception cref="InvalidOperationException">The two amounts are in different currencies.</exception>
    public static Money operator +(Money left, Money right)
    {
        EnsureMatchingCurrencies(left, right);

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// Subtracts one amount from another in the same currency.
    /// </summary>
    /// <param name="left">The amount to subtract from.</param>
    /// <param name="right">The amount to subtract.</param>
    /// <returns>A new <see cref="Money"/> with the difference, which can be negative.</returns>
    /// <exception cref="InvalidOperationException">The two amounts are in different currencies.</exception>
    public static Money operator -(Money left, Money right)
    {
        EnsureMatchingCurrencies(left, right);

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    private static void EnsureMatchingCurrencies(Money left, Money right)
    {
        if (!CurrencyMatch(left, right))
        {
            throw new InvalidOperationException(
                $"Currencies must match, cannot combine {left.Currency} and {right.Currency}");
        }

        return;

        static bool CurrencyMatch(Money left, Money right) => left.Currency == right.Currency;
    }
}
