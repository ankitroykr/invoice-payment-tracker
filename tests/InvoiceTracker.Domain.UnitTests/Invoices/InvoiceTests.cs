using InvoiceTracker.Domain.Common;
using InvoiceTracker.Domain.Invoices;

namespace InvoiceTracker.Domain.UnitTests.Invoices;

public sealed class InvoiceTests
{
    private const int CustomerId = 1;

    private const string Currency = "GBP";

    private const int MinorUnits = 2;

    private static readonly DateOnly IssueDate = new(2026, 11, 1);

    private static readonly DateOnly DueDate = new(2026, 12, 1);

    [Fact]
    public void Create_ReturnsEmptyDraft_WhenCustomerIdAndCurrencyAreValid()
    {
        // Act
        var invoice = Invoice.Create(CustomerId, Currency, MinorUnits);

        // Assert
        Assert.Equal(1, invoice.CustomerId);
        Assert.Equal("GBP", invoice.Currency);
        Assert.Equal(2, invoice.CurrencyMinorUnits);
        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
        Assert.Empty(invoice.Lines);
        Assert.Null(invoice.Number);
        Assert.Null(invoice.IssueDate);
        Assert.Null(invoice.DueDate);
        Assert.Equal(new Money(0m, "GBP"), invoice.Total);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ThrowsArgumentOutOfRangeException_WhenCustomerIdIsZeroOrNegative(int customerId)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => Invoice.Create(customerId, Currency, MinorUnits));

        Assert.Equal(nameof(customerId), exception.ParamName);
    }

    [Fact]
    public void Create_ThrowsArgumentException_WhenCurrencyIsInvalid()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Invoice.Create(CustomerId, "GB", MinorUnits));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public void Create_ThrowsArgumentOutOfRangeException_WhenCurrencyMinorUnitsAreOutOfRange(int currencyMinorUnits)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Invoice.Create(CustomerId, Currency, currencyMinorUnits));
    }

    [Fact]
    public void AddLine_AddsLine_WhenInvoiceIsDraft()
    {
        // Arrange
        const string description = "Senior Engineer Onsite";
        const decimal quantity = 7.5m;
        var unitPrice = new Money(40m, "GBP");
        var invoice = Invoice.Create(CustomerId, Currency, MinorUnits);

        // Act
        var result = invoice.AddLine(description, quantity, unitPrice);

        // Assert
        Assert.True(result.IsSuccess);
        var line = Assert.Single(invoice.Lines);
        Assert.Equal(description, line.Description);
        Assert.Equal(quantity, line.Quantity);
        Assert.Equal(unitPrice.Amount, line.UnitPrice);
    }

    [Fact]
    public void AddLine_ThrowsArgumentException_WhenDescriptionIsEmpty()
    {
        // Arrange
        var invoice = Invoice.Create(CustomerId, Currency, MinorUnits);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => invoice.AddLine("", 2m, new Money(55m, "GBP")));
    }

    [Fact]
    public void AddLine_ThrowsArgumentOutOfRangeException_WhenQuantityIsZero()
    {
        // Arrange
        var invoice = Invoice.Create(CustomerId, Currency, MinorUnits);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => invoice.AddLine("Code review session", 0m, new Money(55m, "GBP")));
    }

    [Fact]
    public void AddLine_ThrowsArgumentOutOfRangeException_WhenUnitPriceIsNegative()
    {
        // Arrange
        var invoice = Invoice.Create(CustomerId, Currency, MinorUnits);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => invoice.AddLine("Discount for late delivery", 1m, new Money(-150m, "GBP")));
    }

    [Fact]
    public void AddLine_ThrowsArgumentException_WhenUnitPriceIsInDifferentCurrency()
    {
        // Arrange
        var invoice = Invoice.Create(CustomerId, Currency, MinorUnits);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => invoice.AddLine("AWS hosting recharge", 1m, new Money(120m, "USD")));

        Assert.Equal("unitPrice", exception.ParamName);
    }

    [Fact]
    public void AddLine_ReturnsNotDraft_WhenInvoiceIsIssued()
    {
        // Arrange
        var invoice = CreateDraftWithLine();
        invoice.Issue(1, IssueDate, DueDate);

        // Act
        var result = invoice.AddLine("Bug fixes after go-live", 3m, new Money(45m, "GBP"));

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.NotDraft, result.Error);
        Assert.Single(invoice.Lines);
    }

    public static TheoryData<string, int, decimal, decimal, decimal> LineRoundingCases => new()
    {
        { "GBP", 2, 1m, 0.125m, 0.13m },
        { "JPY", 0, 2.5m, 100.3m, 251m },
        { "KWD", 3, 3m, 0.1235m, 0.371m }
    };

    [Theory]
    [MemberData(nameof(LineRoundingCases))]
    public void AddLine_RoundsLineTotalHalfAwayFromZero_ToCurrencyMinorUnits(
        string currency, int minorUnits, decimal quantity, decimal unitPrice, decimal expectedTotal)
    {
        // Arrange
        var invoice = Invoice.Create(CustomerId, currency, minorUnits);

        // Act
        invoice.AddLine("Consulting", quantity, new Money(unitPrice, currency));

        // Assert
        Assert.Equal(expectedTotal, Assert.Single(invoice.Lines).Total);
    }

    [Fact]
    public void Total_AddsUpRoundedLineTotals_WhenLinesNeedRounding()
    {
        // Arrange
        var invoice = Invoice.Create(CustomerId, Currency, MinorUnits);
        invoice.AddLine("Printing, first batch", 1m, new Money(0.125m, Currency));
        invoice.AddLine("Printing, second batch", 1m, new Money(0.125m, Currency));

        // Act
        var total = invoice.Total;

        // Assert
        Assert.Equal(new Money(0.26m, Currency), total);
    }

    [Fact]
    public void Total_AddsUpAllLineTotals_WhenInvoiceHasMoreThanOneLine()
    {
        // Arrange
        var invoice = Invoice.Create(CustomerId, Currency, MinorUnits);
        invoice.AddLine("Frontend development", 7.5m, new Money(40m, "GBP"));
        invoice.AddLine("Domain and SSL renewal", 1m, new Money(99.99m, "GBP"));

        // Act
        var total = invoice.Total;

        // Assert
        Assert.Equal(new Money(399.99m, "GBP"), total);
    }

    [Fact]
    public void Issue_SetsNumberDatesAndStatus_WhenDraftHasLines()
    {
        // Arrange
        var invoice = CreateDraftWithLine();

        // Act
        var result = invoice.Issue(1, IssueDate, DueDate);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(new InvoiceNumber(2026, 1), invoice.Number);
        Assert.Equal(new DateOnly(2026, 11, 1), invoice.IssueDate);
        Assert.Equal(new DateOnly(2026, 12, 1), invoice.DueDate);
        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
    }

    [Fact]
    public void Issue_ReturnsNoLines_WhenDraftHasNoLines()
    {
        // Arrange
        var invoice = Invoice.Create(CustomerId, Currency, MinorUnits);

        // Act
        var result = invoice.Issue(1, IssueDate, DueDate);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.NoLines, result.Error);
        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
    }

    [Fact]
    public void Issue_ReturnsNotDraft_WhenInvoiceIsAlreadyIssued()
    {
        // Arrange
        var invoice = CreateDraftWithLine();
        invoice.Issue(1, IssueDate, DueDate);

        // Act
        var result = invoice.Issue(2, IssueDate, DueDate);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.NotDraft, result.Error);
        Assert.Equal(new InvoiceNumber(2026, 1), invoice.Number);
    }

    [Fact]
    public void Issue_ThrowsArgumentOutOfRangeException_WhenDueDateIsBeforeIssueDate()
    {
        // Arrange
        var invoice = CreateDraftWithLine();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => invoice.Issue(1, IssueDate, new DateOnly(2026, 10, 31)));
    }

    [Fact]
    public void Void_SetsStatusToVoid_WhenInvoiceIsIssued()
    {
        // Arrange
        var invoice = CreateDraftWithLine();
        invoice.Issue(1, IssueDate, DueDate);

        // Act
        var result = invoice.Void();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(InvoiceStatus.Void, invoice.Status);
    }

    [Fact]
    public void Void_ReturnsNotIssued_WhenInvoiceIsStillDraft()
    {
        // Arrange
        var invoice = CreateDraftWithLine();

        // Act
        var result = invoice.Void();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.NotIssued, result.Error);
        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
    }

    [Fact]
    public void Void_ReturnsNotIssued_WhenInvoiceIsAlreadyVoid()
    {
        // Arrange
        var invoice = CreateDraftWithLine();
        invoice.Issue(1, IssueDate, DueDate);
        invoice.Void();

        // Act
        var result = invoice.Void();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InvoiceErrors.NotIssued, result.Error);
    }

    private static Invoice CreateDraftWithLine()
    {
        var invoice = Invoice.Create(CustomerId, Currency, MinorUnits);
        invoice.AddLine("website work exploration", 7.5m, new Money(38m, Currency));
        return invoice;
    }
}
