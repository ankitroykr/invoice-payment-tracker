using FluentValidation.TestHelper;

using InvoiceTracker.Application.Customers.CreateCustomer;
using InvoiceTracker.Domain.Customers;

namespace InvoiceTracker.Application.UnitTests.Customers.CreateCustomer;

public sealed class CreateCustomerCommandValidatorTests
{
    private const string ValidName = "Tester Test";

    private const string ValidEmail = "tester.test@example.com";

    private readonly CreateCustomerCommandValidator _validator = new();

    [Fact]
    public void Validate_HasNoErrors_WhenNameAndEmailAreValid()
    {
        // Arrange
        var command = new CreateCustomerCommand(ValidName, ValidEmail);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_HasNameError_WhenNameIsInvalid(string? name)
    {
        // Arrange
        var command = new CreateCustomerCommand(name!, ValidEmail);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_HasEmailError_WhenEmailIsInvalid(string? email)
    {
        // Arrange
        var command = new CreateCustomerCommand(ValidName, email!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_HasNameError_WhenNameIsTooLong()
    {
        // Arrange
        var name = new string('c', Customer.NameMaxLength + 1);
        var command = new CreateCustomerCommand(name, ValidEmail);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_HasNoNameError_WhenNameIsAtMaxLength()
    {
        // Arrange
        var name = new string('c', Customer.NameMaxLength);
        var command = new CreateCustomerCommand(name, ValidEmail);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_HasEmailError_WhenEmailIsTooLong()
    {
        // Arrange
        var email = "@example.com".PadLeft(Customer.EmailMaxLength + 1, 'e');
        var command = new CreateCustomerCommand(ValidName, email);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_HasNoEmailError_WhenEmailIsAtMaxLength()
    {
        // Arrange
        var email = "@example.com".PadLeft(Customer.EmailMaxLength, 'e');
        var command = new CreateCustomerCommand(ValidName, email);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("tester.example.com")]
    [InlineData("@example.com")]
    [InlineData("tester@")]
    [InlineData("tester@@example.com")]
    public void Validate_HasEmailError_WhenEmailIsNotAnEmailAddress(string email)
    {
        // Arrange
        var command = new CreateCustomerCommand(ValidName, email);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
