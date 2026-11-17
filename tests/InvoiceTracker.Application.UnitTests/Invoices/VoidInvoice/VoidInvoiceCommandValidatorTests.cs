using FluentValidation.TestHelper;

using InvoiceTracker.Application.Invoices.VoidInvoice;

namespace InvoiceTracker.Application.UnitTests.Invoices.VoidInvoice;

public sealed class VoidInvoiceCommandValidatorTests
{
    private const int InvalidInvoiceId = 0;

    private const int ValidInvoiceId = 1;

    private readonly VoidInvoiceCommandValidator _validator = new();

    [Fact]
    public void Validate_HasNoErrors_WhenCommandIsValid()
    {
        // Arrange
        var command = new VoidInvoiceCommand(ValidInvoiceId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_HasInvoiceIdError_WhenInvoiceIdIsNotPositive()
    {
        // Arrange
        var command = new VoidInvoiceCommand(InvalidInvoiceId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(i => i.InvoiceId);
    }
}
