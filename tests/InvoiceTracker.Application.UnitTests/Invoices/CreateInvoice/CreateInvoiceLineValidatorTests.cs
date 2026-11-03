using FluentValidation.TestHelper;

using InvoiceTracker.Application.Invoices.CreateInvoice;
using InvoiceTracker.Domain.Invoices;

namespace InvoiceTracker.Application.UnitTests.Invoices.CreateInvoice;

public sealed class CreateInvoiceLineValidatorTests
{
    private const string ValidDescription = "Website redesign";

    private const decimal ValidQuantity = 2m;

    private const decimal ValidUnitPrice = 150m;

    private readonly CreateInvoiceLineValidator _validator = new();

    [Fact]
    public void Validate_HasNoErrors_WhenLineIsValid()
    {
        // Arrange
        var line = new CreateInvoiceLine(ValidDescription, ValidQuantity, ValidUnitPrice);

        // Act
        var result = _validator.TestValidate(line);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_HasDescriptionError_WhenDescriptionIsNullOrWhiteSpace(string? description)
    {
        // Arrange
        var line = new CreateInvoiceLine(description!, ValidQuantity, ValidUnitPrice);

        // Act
        var result = _validator.TestValidate(line);

        // Assert
        result.ShouldHaveValidationErrorFor(invoiceLine => invoiceLine.Description);
    }

    [Fact]
    public void Validate_HasDescriptionError_WhenDescriptionIsTooLong()
    {
        // Arrange
        var description = new string('a', InvoiceLine.DescriptionMaxLength + 1);
        var line = new CreateInvoiceLine(description, ValidQuantity, ValidUnitPrice);

        // Act
        var result = _validator.TestValidate(line);

        // Assert
        result.ShouldHaveValidationErrorFor(invoiceLine => invoiceLine.Description);
    }

    [Fact]
    public void Validate_HasNoDescriptionError_WhenDescriptionIsAtMaxLength()
    {
        // Arrange
        var description = new string('a', InvoiceLine.DescriptionMaxLength);
        var line = new CreateInvoiceLine(description, ValidQuantity, ValidUnitPrice);

        // Act
        var result = _validator.TestValidate(line);

        // Assert
        result.ShouldNotHaveValidationErrorFor(invoiceLine => invoiceLine.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_HasQuantityError_WhenQuantityIsNotPositive(decimal quantity)
    {
        // Arrange
        var line = new CreateInvoiceLine(ValidDescription, quantity, ValidUnitPrice);

        // Act
        var result = _validator.TestValidate(line);

        // Assert
        result.ShouldHaveValidationErrorFor(invoiceLine => invoiceLine.Quantity);
    }

    [Fact]
    public void Validate_HasQuantityError_WhenQuantityHasMoreThanFourDecimalPlaces()
    {
        // Arrange
        var line = new CreateInvoiceLine(ValidDescription, 1.12345m, ValidUnitPrice);

        // Act
        var result = _validator.TestValidate(line);

        // Assert
        result.ShouldHaveValidationErrorFor(invoiceLine => invoiceLine.Quantity);
    }

    [Fact]
    public void Validate_HasNoQuantityError_WhenQuantityIsSmallestAllowed()
    {
        // Arrange
        var line = new CreateInvoiceLine(ValidDescription, 0.0001m, ValidUnitPrice);

        // Act
        var result = _validator.TestValidate(line);

        // Assert
        result.ShouldNotHaveValidationErrorFor(invoiceLine => invoiceLine.Quantity);
    }

    [Fact]
    public void Validate_HasUnitPriceError_WhenUnitPriceIsNegative()
    {
        // Arrange
        var line = new CreateInvoiceLine(ValidDescription, ValidQuantity, -1m);

        // Act
        var result = _validator.TestValidate(line);

        // Assert
        result.ShouldHaveValidationErrorFor(invoiceLine => invoiceLine.UnitPrice);
    }

    [Fact]
    public void Validate_HasNoUnitPriceError_WhenUnitPriceIsZero()
    {
        // Arrange
        var line = new CreateInvoiceLine(ValidDescription, ValidQuantity, 0m);

        // Act
        var result = _validator.TestValidate(line);

        // Assert
        result.ShouldNotHaveValidationErrorFor(invoiceLine => invoiceLine.UnitPrice);
    }

    [Fact]
    public void Validate_HasUnitPriceError_WhenUnitPriceHasMoreThanFourDecimalPlaces()
    {
        // Arrange
        var line = new CreateInvoiceLine(ValidDescription, ValidQuantity, 1.12345m);

        // Act
        var result = _validator.TestValidate(line);

        // Assert
        result.ShouldHaveValidationErrorFor(invoiceLine => invoiceLine.UnitPrice);
    }
}
