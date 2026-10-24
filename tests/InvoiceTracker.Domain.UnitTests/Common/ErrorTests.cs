using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.UnitTests.Common;

public sealed class ErrorTests
{
    [Theory]
    [InlineData(null, "message")]
    [InlineData("code", null)]
    public void Constructor_ThrowsArgumentNullException_WhenCodeOrMessageIsNull(string? code, string? message)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Error(code!, message!));
    }

    [Theory]
    [InlineData("", "message")]
    [InlineData("code", "")]
    [InlineData("code", "   ")]
    [InlineData("   ", "message")]
    public void Constructor_ThrowsArgumentException_WhenCodeOrMessageIsEmptyOrWhiteSpaces(string code, string message)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Error(code, message));
    }

    [Fact]
    public void Constructor_SetsCodeAndMessage_WhenBothAreValid()
    {
        // Arrange
        const string code = "code";
        const string message = "message";

        // Act
        var error = new Error(code, message);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(message, error.Message);
    }
}
