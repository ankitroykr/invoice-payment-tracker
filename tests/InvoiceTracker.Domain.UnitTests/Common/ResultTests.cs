using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.UnitTests.Common;

public sealed class ResultTests
{
    [Fact]
    public void Success_ReturnsSuccessfulResultWithoutError()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_ReturnsFailedResultWithError_WhenErrorIsProvided()
    {
        // Arrange
        var error = Error.Validation("code", "message");

        // Act
        var result = Result.Failure(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Failure_ThrowsArgumentNullException_WhenErrorIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Result.Failure(null!));
    }
}
