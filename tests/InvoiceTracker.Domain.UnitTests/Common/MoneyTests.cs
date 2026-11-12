using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.UnitTests.Common;

public sealed class MoneyTests
{
    private const string Currency = "GBP";

    public static TheoryData<decimal, int, decimal> RoundingCases => new()
    {
        { 279.99975m, 2, 280m },
        { 0.125m, 2, 0.13m },
        { -0.125m, 2, -0.13m },
        { 1234.5m, 0, 1235m },
        { 1.2345m, 3, 1.235m }
    };

    [Fact]
    public void Constructor_SetsAmountAndCurrency_WhenCurrencyIsValid()
    {
        // Arrange
        const decimal amount = 10;

        // Act
        var money = new Money(amount, Currency);

        // Assert
        Assert.Equal(amount, money.Amount);
        Assert.Equal(Currency, money.Currency);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("INVALID")]
    [InlineData("G5P")]
    [InlineData("GB")]
    [InlineData("GbP")]
    public void Constructor_ThrowsArgumentException_WhenCurrencyIsInvalid(string currency)
    {
        // Arrange
        const decimal amount = 10;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Money(amount, currency));
    }

    [Fact]
    public void Constructor_SetsAmountAndCurrency_WhenAmountIsNegative()
    {
        // Arrange
        const decimal amount = -10;

        // Act
        var money = new Money(amount, Currency);

        // Assert
        Assert.Equal(amount, money.Amount);
        Assert.Equal(Currency, money.Currency);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenCurrencyIsNull()
    {
        // Arrange
        const decimal amount = 10;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Money(amount, null!));
    }

    [Fact]
    public void Zero_ReturnsZeroAmount_InGivenCurrency()
    {
        // Act
        var money = Money.Zero(Currency);

        // Assert
        Assert.Equal(0, money.Amount);
        Assert.Equal(Currency, money.Currency);
    }

    [Fact]
    public void Equals_ReturnsTrue_WhenAmountAndCurrencyMatch()
    {
        // Arrange
        var moneyOne = new Money(10, Currency);
        var moneyTwo = new Money(10.00m, Currency);

        // Act
        var areEqual = moneyOne.Equals(moneyTwo);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenCurrencyDiffers()
    {
        // Arrange
        var moneyOne = new Money(10, Currency);
        var moneyTwo = new Money(10.00m, "EUR");

        // Act
        var areEqual = moneyOne.Equals(moneyTwo);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void Add_ReturnsSum_WhenCurrenciesMatch()
    {
        // Arrange
        var moneyOne = new Money(10, Currency);
        var moneyTwo = new Money(5, Currency);

        // Act
        var sum = moneyOne + moneyTwo;

        // Assert
        Assert.Equal(new Money(15, Currency), sum);
    }

    [Fact]
    public void Subtract_ReturnsDifference_WhenCurrenciesMatch()
    {
        // Arrange
        var moneyOne = new Money(10, Currency);
        var moneyTwo = new Money(5, Currency);

        // Act
        var difference = moneyOne - moneyTwo;

        // Assert
        Assert.Equal(new Money(5, Currency), difference);
    }

    [Fact]
    public void Add_ThrowsInvalidOperationException_WhenCurrenciesDiffer()
    {
        // Arrange
        var moneyOne = new Money(10, Currency);
        var moneyTwo = new Money(5, "EUR");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => moneyOne + moneyTwo);
    }

    [Fact]
    public void Subtract_ThrowsInvalidOperationException_WhenCurrenciesDiffer()
    {
        // Arrange
        var moneyOne = new Money(10, Currency);
        var moneyTwo = new Money(5, "EUR");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => moneyOne - moneyTwo);
    }

    [Theory]
    [MemberData(nameof(RoundingCases))]
    public void Round_RoundsHalfAwayFromZero_ToGivenMinorUnits(decimal amount, int minorUnits, decimal expected)
    {
        // Arrange
        var money = new Money(amount, Currency);

        // Act
        var rounded = money.Round(minorUnits);

        // Assert
        Assert.Equal(new Money(expected, Currency), rounded);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public void Round_ThrowsArgumentOutOfRangeException_WhenMinorUnitsAreOutOfRange(int minorUnits)
    {
        // Arrange
        var money = new Money(10m, Currency);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => money.Round(minorUnits));
    }

    [Theory]
    [InlineData("GBP", true)]
    [InlineData("gbp", false)]
    [InlineData("GB", false)]
    [InlineData("GBPP", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidCode_ReturnsTrue_OnlyWhenCodeIsThreeUppercaseLetters(string? code, bool expected)
    {
        // Act
        var isValid = Money.IsValidCode(code);

        // Assert
        Assert.Equal(expected, isValid);
    }
}
