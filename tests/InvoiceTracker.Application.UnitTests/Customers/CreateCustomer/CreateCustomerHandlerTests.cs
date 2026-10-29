using InvoiceTracker.Application.Customers.CreateCustomer;
using InvoiceTracker.Domain.Common;
using InvoiceTracker.Domain.Customers;

using NSubstitute;

namespace InvoiceTracker.Application.UnitTests.Customers.CreateCustomer;

public sealed class CreateCustomerHandlerTests
{
    private const string ValidName = "Tester Test";

    private const string ValidEmail = "tester.test@example.com";

    private readonly ICustomerRepository _customerRepository = Substitute.For<ICustomerRepository>();

    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly CreateCustomerCommandValidator _createCustomerCommandValidator = new();

    private readonly CreateCustomerHandler _handler;

    public CreateCustomerHandlerTests()
    {
        _handler = new CreateCustomerHandler(_createCustomerCommandValidator, _customerRepository, _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_AddsCustomerAndSaves_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateCustomerCommand(ValidName, ValidEmail);

        // Act
        await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        _customerRepository.Received(1).Add(Arg.Is<Customer>(c => c.Name == ValidName && c.Email == ValidEmail));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsValidationErrorWithFieldErrors_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new CreateCustomerCommand(string.Empty, ValidEmail);

        // Act
        var result = await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error!.Type);
        Assert.Contains(nameof(CreateCustomerCommand.Name), result.Error!.FieldErrors);
    }

    [Fact]
    public async Task HandleAsync_DoesNotAddOrSave_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new CreateCustomerCommand(string.Empty, ValidEmail);

        // Act
        await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        _customerRepository.DidNotReceive().Add(Arg.Any<Customer>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateCustomerCommand(ValidName, ValidEmail);

        // Act
        var result = await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
