using System.Globalization;

namespace InvoiceTracker.Domain.Common;

public sealed record Money
{
    public decimal Amount { get; }

    public string Currency { get; }

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

    public static Money Zero(string currency) => new(0, currency);

    public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"{Amount} {Currency}");

    public static Money operator +(Money left, Money right)
    {
        EnsureMatchingCurrencies(left, right);

        return new Money(left.Amount + right.Amount, left.Currency);
    }

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
