using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.UnitTests.Common;

public sealed class ErrorTests
{
    private const string Code = "code";

    private const string Message = "message";

    public static TheoryData<Func<string, string, Error>, ErrorType> ErrorFactories => new()
    {
        { Error.Validation, ErrorType.Validation },
        { Error.NotFound, ErrorType.NotFound },
        { Error.BusinessRule, ErrorType.BusinessRule },
        { Error.Conflict, ErrorType.Conflict }
    };

    [Theory]
    [InlineData(null, "message")]
    [InlineData("code", null)]
    public void Factory_ThrowsArgumentNullException_WhenCodeOrMessageIsNull(string? code, string? message)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Error.Validation(code!, message!));
    }

    [Theory]
    [InlineData("", "message")]
    [InlineData("code", "")]
    [InlineData("code", "   ")]
    [InlineData("   ", "message")]
    public void Factory_ThrowsArgumentException_WhenCodeOrMessageIsEmptyOrWhiteSpaces(string code, string message)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Error.NotFound(code, message));
    }

    [Fact]
    public void Factory_SetsCodeAndMessage_WhenBothAreValid()
    {
        // Act
        var error = Error.BusinessRule(Code, Message);

        // Assert
        Assert.Equal(Code, error.Code);
        Assert.Equal(Message, error.Message);
    }

    [Theory]
    [MemberData(nameof(ErrorFactories))]
    public void Factory_SetsCodeMessageAndMatchingErrorType_WhenCodeAndMessageAreValid(
        Func<string, string, Error> errorFactory,
        ErrorType expectedErrorType)
    {
        // Act
        var error = errorFactory(Code, Message);

        // Assert
        Assert.Equal(Code, error.Code);
        Assert.Equal(Message, error.Message);
        Assert.Equal(expectedErrorType, error.Type);
    }

    [Fact]
    public void Factory_SetsEmptyFieldErrors_WhenNoFieldErrorsAreGiven()
    {
        // Act
        var error = Error.Validation(Code, Message);

        // Assert
        Assert.Empty(error.FieldErrors);
    }

    [Fact]
    public void Validation_SetsFieldErrors_WhenFieldErrorsAreGiven()
    {
        // Arrange
        var fieldErrors = new Dictionary<string, string[]> { ["Email"] = ["'Email' is not a valid email address."] };

        // Act
        var error = Error.Validation(Code, Message, fieldErrors);

        // Assert
        Assert.Equal(fieldErrors, error.FieldErrors);
    }

    [Fact]
    public void Validation_ThrowsArgumentNullException_WhenFieldErrorsIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Error.Validation(Code, Message, null!));
    }

    [Fact]
    public void Validation_ThrowsArgumentException_WhenFieldErrorsIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Error.Validation(Code, Message, new Dictionary<string, string[]>()));
    }
}
