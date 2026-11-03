using FluentValidation.TestHelper;

using InvoiceTracker.Application.Invoices.CreateInvoice;

namespace InvoiceTracker.Application.UnitTests.Invoices.CreateInvoice;

public sealed class CreateInvoiceCommandValidatorTests
{
    private const int ValidCustomerId = 1;

    private const string ValidCurrency = "GBP";

    private static readonly CreateInvoiceLine ValidLine = new("Website redesign", 6.5m, 47m);

    private readonly CreateInvoiceCommandValidator _validator = new();

    [Fact]
    public void Validate_HasNoErrors_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateInvoiceCommand(ValidCustomerId, ValidCurrency, [ValidLine]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_HasCustomerIdError_WhenCustomerIdIsNotPositive(int customerId)
    {
        // Arrange
        var command = new CreateInvoiceCommand(customerId, ValidCurrency, [ValidLine]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.CustomerId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("GB")]
    [InlineData("GBPX")]
    [InlineData("gbp")]
    [InlineData("G1P")]
    public void Validate_HasCurrencyError_WhenCurrencyIsInvalid(string? currency)
    {
        // Arrange
        var command = new CreateInvoiceCommand(ValidCustomerId, currency!, [ValidLine]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Currency);
    }

    [Fact]
    public void Validate_HasCurrencyFormatMessage_WhenCurrencyIsNotThreeUppercaseLetters()
    {
        // Arrange
        var command = new CreateInvoiceCommand(ValidCustomerId, "gbp", [ValidLine]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Currency)
            .WithErrorMessage("'Currency' must be a three-letter uppercase ISO 4217 code, for example, GBP.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_HasOnlyNotEmptyError_WhenCurrencyIsMissing(string? currency)
    {
        // Arrange
        var command = new CreateInvoiceCommand(ValidCustomerId, currency!, [ValidLine]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        var error = Assert.Single(result.ShouldHaveValidationErrorFor(command => command.Currency));
        Assert.Equal("'Currency' must not be empty.", error.ErrorMessage);
    }

    [Fact]
    public void Validate_HasLinesError_WhenLinesIsEmpty()
    {
        // Arrange
        var command = new CreateInvoiceCommand(ValidCustomerId, ValidCurrency, []);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Lines);
    }

    [Fact]
    public void Validate_HasLineDescriptionError_WhenLineDescriptionIsEmpty()
    {
        // Arrange
        var line = ValidLine with { Description = string.Empty };
        var command = new CreateInvoiceCommand(ValidCustomerId, ValidCurrency, [line]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Lines[0].Description");
    }
}
