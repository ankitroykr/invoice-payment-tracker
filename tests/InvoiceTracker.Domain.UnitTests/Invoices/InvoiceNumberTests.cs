using InvoiceTracker.Domain.Invoices;

namespace InvoiceTracker.Domain.UnitTests.Invoices;

public sealed class InvoiceNumberTests
{
    private const int ValidYear = 2026;

    private const int ValidSequence = 1;

    [Fact]
    public void Constructor_SetsYearAndSequence_WhenBothAreValid()
    {
        // Act
        var invoiceNumber = new InvoiceNumber(ValidYear, ValidSequence);

        // Assert
        Assert.Equal(ValidYear, invoiceNumber.Year);
        Assert.Equal(ValidSequence, invoiceNumber.Sequence);
    }

    [Theory]
    [InlineData(2026, 1, "INV-2026-0001")]
    [InlineData(2027, 9999, "INV-2027-9999")]
    [InlineData(2029, 39, "INV-2029-0039")]
    public void ToString_ReturnsInvoiceNumberWithSequenceNumberPaddedToFourDigits_WhenSequenceIsShorter(int year,
        int sequence, string expected)
    {
        // Act
        var invoiceNumber = new InvoiceNumber(year, sequence);

        // Assert
        Assert.Equal(expected, invoiceNumber.ToString());
    }

    [Theory]
    [InlineData(2026, 13423, "INV-2026-13423")]
    [InlineData(2027, 568524, "INV-2027-568524")]
    [InlineData(2029, 254365789, "INV-2029-254365789")]
    public void ToString_KeepsAllSequenceNumberDigits_WhenSequenceIsLongerThanFourDigits(int year,
        int sequence, string expected)
    {
        // Act
        var invoiceNumber = new InvoiceNumber(year, sequence);

        // Assert
        Assert.Equal(expected, invoiceNumber.ToString());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Constructor_ThrowsArgumentOutOfRangeException_WhenSequenceIsLessThanOne(int sequence)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new InvoiceNumber(ValidYear, sequence));

        Assert.Equal(nameof(sequence), exception.ParamName);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(9999)]
    public void Constructor_SetsYear_WhenYearIsAtDateOnlyBoundary(int year)
    {
        // Act
        var invoiceNumber = new InvoiceNumber(year, ValidSequence);

        // Assert
        Assert.Equal(year, invoiceNumber.Year);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(10000)]
    public void Constructor_ThrowsArgumentOutOfRangeException_WhenYearIsOutOfRange(int year)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new InvoiceNumber(year, ValidSequence));

        Assert.Equal(nameof(year), exception.ParamName);
    }
}
