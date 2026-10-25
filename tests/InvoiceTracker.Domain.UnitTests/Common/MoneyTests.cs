using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.UnitTests.Common;

public sealed class MoneyTests
{
    [Fact]
    public void Constructor_SetsAmountAndCurrency_WhenCurrencyIsValid()
    {
        // Arrange
        const decimal amount = 10;
        const string currency = "GBP";

        // Act
        var money = new Money(amount, currency);

        // Assert
        Assert.Equal(amount, money.Amount);
        Assert.Equal(currency, money.Currency);
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
        const string currency = "GBP";

        // Act
        var money = new Money(amount, currency);

        // Assert
        Assert.Equal(amount, money.Amount);
        Assert.Equal(currency, money.Currency);
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
    public void Equals_ReturnsTrue_WhenAmountAndCurrencyMatch()
    {
        // Arrange
        var moneyOne = new Money(10, "GBP");
        var moneyTwo = new Money(10.00m, "GBP");

        // Act
        var areEqual = moneyOne.Equals(moneyTwo);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenCurrencyDiffers()
    {
        // Arrange
        var moneyOne = new Money(10, "GBP");
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
        var moneyOne = new Money(10, "GBP");
        var moneyTwo = new Money(5, "GBP");

        // Act
        var sum = moneyOne + moneyTwo;

        // Assert
        Assert.Equal(new Money(15, "GBP"), sum);
    }

    [Fact]
    public void Subtract_ReturnsDifference_WhenCurrenciesMatch()
    {
        // Arrange
        var moneyOne = new Money(10, "GBP");
        var moneyTwo = new Money(5, "GBP");

        // Act
        var difference = moneyOne - moneyTwo;

        // Assert
        Assert.Equal(new Money(5, "GBP"), difference);
    }

    [Fact]
    public void Add_ThrowsInvalidOperationException_WhenCurrenciesDiffer()
    {
        // Arrange
        var moneyOne = new Money(10, "GBP");
        var moneyTwo = new Money(5, "EUR");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => moneyOne + moneyTwo);
    }

    [Fact]
    public void Subtract_ThrowsInvalidOperationException_WhenCurrenciesDiffer()
    {
        // Arrange
        var moneyOne = new Money(10, "GBP");
        var moneyTwo = new Money(5, "EUR");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => moneyOne - moneyTwo);
    }
}
