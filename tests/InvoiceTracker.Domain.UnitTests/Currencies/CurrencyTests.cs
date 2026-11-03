using InvoiceTracker.Domain.Currencies;

namespace InvoiceTracker.Domain.UnitTests.Currencies;

public sealed class CurrencyTests
{
    [Fact]
    public void Create_SetsCodeMinorUnitsAndName_WhenValuesAreValid()
    {
        // Act
        var currency = Currency.Create("GBP", 2, "Pound Sterling");

        // Assert
        Assert.Equal("GBP", currency.Code);
        Assert.Equal(2, currency.MinorUnits);
        Assert.Equal("Pound Sterling", currency.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("GB")]
    [InlineData("gbp")]
    [InlineData("G1P")]
    public void Create_ThrowsArgumentException_WhenCodeIsInvalid(string code)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Currency.Create(code, 2, "Pound Sterling"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public void Create_ThrowsArgumentOutOfRangeException_WhenMinorUnitsAreOutOfRange(int minorUnits)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Currency.Create("GBP", minorUnits, "Pound Sterling"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsArgumentException_WhenNameIsEmptyOrWhiteSpace(string name)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Currency.Create("GBP", 2, name));
    }

    [Fact]
    public void Create_ThrowsArgumentException_WhenNameIsTooLong()
    {
        // Arrange
        var name = new string('a', Currency.NameMaxLength + 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Currency.Create("GBP", 2, name));
    }

    [Fact]
    public void Create_ThrowsArgumentExceptionWithCodeParamName_WhenCodeIsInvalid()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Currency.Create("IN", 2, "Indian Rupee"));
        Assert.Equal("code", exception.ParamName);
    }
}
