using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace InvoiceTracker.Domain.Common;

/// <summary>
/// An amount of money in a specific currency. It can't be changed once created, and the amount can be negative.
/// </summary>
public sealed record Money
{
    /// <summary>
    /// The most decimal places any ISO 4217 currency uses.
    /// </summary>
    public const int MaxMinorUnits = 4;

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
        ThrowIfInvalidCode(currency);

        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Checks whether a currency code has the ISO 4217 format: three uppercase letters, for example, GBP.
    /// It only checks the format, not whether the currency actually exists.
    /// </summary>
    /// <param name="code">The code to check. Null is allowed and returns false.</param>
    /// <returns><see langword="true"/> if the code is three uppercase letters; otherwise <see langword="false"/>.</returns>
    public static bool IsValidCode(string? code) => code is { Length: 3 } && code.All(char.IsAsciiLetterUpper);

    /// <summary>
    /// Throws if a currency code doesn't have the ISO 4217 format: three uppercase letters, for example, GBP.
    /// It only checks the format, not whether the currency actually exists.
    /// </summary>
    /// <param name="currencyCode">The code to check.</param>
    /// <param name="paramName">The name of the caller's argument. Leave it out; the compiler fills it in.</param>
    /// <exception cref="ArgumentNullException">The code is null.</exception>
    /// <exception cref="ArgumentException">The code is empty, whitespace, or not three uppercase letters.</exception>
    public static void ThrowIfInvalidCode([NotNull] string? currencyCode,
        [CallerArgumentExpression(nameof(currencyCode))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode, paramName);

        if (!IsValidCode(currencyCode))
        {
            throw new ArgumentException(
                $"Currency code '{currencyCode}' is not a valid ISO 4217 code; expected 3 uppercase letters such as GBP.",
                paramName);
        }
    }

    /// <summary>
    /// Rounds the amount to the currency's minor units, with halves rounded away from zero.
    /// </summary>
    /// <param name="minorUnits">The number of decimal places the currency uses under ISO 4217, for example, 2 for GBP.</param>
    /// <returns>A new <see cref="Money"/> with the rounded amount, in the same currency.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The minor units are below 0 or above 4.</exception>
    public Money Round(int minorUnits)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minorUnits);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minorUnits, MaxMinorUnits);

        return new Money(Math.Round(Amount, minorUnits, MidpointRounding.AwayFromZero), Currency);
    }

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
