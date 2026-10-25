using InvoiceTracker.Domain.Customers;

namespace InvoiceTracker.Domain.UnitTests.Customers;

public sealed class CustomerTests
{
    [Fact]
    public void Create_SetsNameAndEmail_WhenBothAreValid()
    {
        // Arrange
        const string name = "Test Test";
        const string email = "test.test@example.com";

        // Act
        var customer = Customer.Create(name, email);

        // Assert
        Assert.Equal(name, customer.Name);
        Assert.Equal(email, customer.Email);
    }

    [Fact]
    public void Create_CustomerIdSetAsZero_WhenNewCustomer()
    {
        // Act
        var customer = Customer.Create("Test Test", "test.test@example.com");

        // Assert
        Assert.Equal(0, customer.Id);
    }

    [Theory]
    [InlineData("", "test.test@example.com")]
    [InlineData(" ", "test.test@example.com")]
    [InlineData("Test Test", "")]
    [InlineData("Test Test", " ")]
    public void Create_ThrowsArgumentException_WhenNameOrEmailIsEmptyOrWhitespace(string name, string email)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Customer.Create(name, email));
    }

    [Theory]
    [InlineData(null, "test.test@example.com")]
    [InlineData("Test Test", null)]
    public void Create_ThrowsArgumentNullException_WhenNameOrEmailIsNull(string? name, string? email)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Customer.Create(name!, email!));
    }
}
