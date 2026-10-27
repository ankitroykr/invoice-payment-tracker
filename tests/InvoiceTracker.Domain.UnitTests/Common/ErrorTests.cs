using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.UnitTests.Common;

public sealed class ErrorTests
{
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
        // Arrange
        const string code = "code";
        const string message = "message";

        // Act
        var error = Error.BusinessRule(code, message);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(message, error.Message);
    }

    [Theory]
    [MemberData(nameof(ErrorFactories))]
    public void Factory_SetsCodeMessageAndMatchingErrorType_WhenCodeAndMessageAreValid(Func<string, string, Error> errorFactory,
        ErrorType expectedErrorType)
    {
        // Arrange
        const string code = "code";
        const string message = "message";

        // Act
        var error = errorFactory(code, message);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(message, error.Message);
        Assert.Equal(expectedErrorType, error.Type);
    }
}
