using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.UnitTests.Common;

public sealed class ResultOfTTests
{
    [Fact]
    public void Success_ReturnsSuccessfulResultWithValue()
    {
        // Arrange
        const string resultValue = "test";

        // Act
        var result = Result.Success(resultValue);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(resultValue, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_ReturnsFailedResultWithError_WhenErrorIsProvided()
    {
        // Arrange
        const string code = "code";
        const string message = "message";
        var error = new Error(code, message);

        // Act
        var result = Result.Failure<string>(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Value_ThrowsInvalidOperationException_WhenResultIsFailure()
    {
        // Arrange
        var result = Result.Failure<string>(new Error("code", "message"));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Failure_ThrowsArgumentNullException_WhenErrorIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Result.Failure<string>(null!));
    }
}
