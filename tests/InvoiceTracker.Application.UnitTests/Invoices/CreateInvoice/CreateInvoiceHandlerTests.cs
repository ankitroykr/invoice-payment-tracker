using InvoiceTracker.Application.Currencies;
using InvoiceTracker.Application.Invoices.CreateInvoice;
using InvoiceTracker.Domain.Common;
using InvoiceTracker.Domain.Customers;
using InvoiceTracker.Domain.Invoices;

using NSubstitute;

namespace InvoiceTracker.Application.UnitTests.Invoices.CreateInvoice;

public sealed class CreateInvoiceHandlerTests
{
    private const int ValidCustomerId = 1;

    private const string Currency = "GBP";

    private const string UnsupportedCurrency = "CHF";

    private static readonly CreateInvoiceCommand ValidCommand = new(ValidCustomerId, Currency,
    [
        new CreateInvoiceLine("Website redesign", 4m, 55m),
        new CreateInvoiceLine("Hosting for one month", 6m, 15m)
    ]);

    private readonly IInvoiceRepository _invoiceRepository = Substitute.For<IInvoiceRepository>();

    private readonly ICustomerRepository _customerRepository = Substitute.For<ICustomerRepository>();

    private readonly ICurrencyProvider _currencyProvider = Substitute.For<ICurrencyProvider>();

    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly CreateInvoiceCommandValidator _createInvoiceCommandValidator = new();

    private readonly CreateInvoiceHandler _handler;

    public CreateInvoiceHandlerTests()
    {
        _currencyProvider.FindAsync(Currency, Arg.Any<CancellationToken>())
            .Returns(new SupportedCurrency(Currency, 2, "Pound Sterling"));

        _handler = new CreateInvoiceHandler(_createInvoiceCommandValidator, _invoiceRepository, _customerRepository,
            _currencyProvider, _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenCommandIsValid()
    {
        // Arrange
        _customerRepository.GetByIdAsync(ValidCustomerId, Arg.Any<CancellationToken>())
            .Returns(Customer.Create("Test Customer", "test@example.com"));

        // Act
        var result = await _handler.HandleAsync(ValidCommand, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task HandleAsync_AddsDraftInvoiceWithLinesAndSaves_WhenCommandIsValid()
    {
        // Arrange
        _customerRepository.GetByIdAsync(ValidCustomerId, Arg.Any<CancellationToken>())
            .Returns(Customer.Create("Test Customer", "test@example.com"));

        // Act
        await _handler.HandleAsync(ValidCommand, TestContext.Current.CancellationToken);

        // Assert
        _invoiceRepository.Received(1).Add(Arg.Is<Invoice>(i =>
            i.CustomerId == ValidCustomerId &&
            i.Currency == Currency &&
            i.CurrencyMinorUnits == 2 &&
            i.Status == InvoiceStatus.Draft &&
            i.Lines.Count == 2 &&
            i.Total == new Money(310m, Currency)));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsCustomerNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        _customerRepository.GetByIdAsync(ValidCustomerId, Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        // Act
        var result = await _handler.HandleAsync(ValidCommand, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(InvoiceErrors.CustomerNotFound, result.Error);
    }

    [Fact]
    public async Task HandleAsync_DoesNotAddOrSave_WhenCustomerDoesNotExist()
    {
        // Arrange
        _customerRepository.GetByIdAsync(ValidCustomerId, Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        // Act
        await _handler.HandleAsync(ValidCommand, TestContext.Current.CancellationToken);

        // Assert
        _invoiceRepository.DidNotReceive().Add(Arg.Any<Invoice>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsCurrencyNotSupported_WhenCurrencyIsNotSupported()
    {
        // Arrange
        var command = ValidCommand with { Currency = UnsupportedCurrency };

        // Act
        var result = await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(InvoiceErrors.CurrencyNotSupported, result.Error);
    }

    [Fact]
    public async Task HandleAsync_DoesNotLookUpCustomerOrSave_WhenCurrencyIsNotSupported()
    {
        // Arrange
        var command = ValidCommand with { Currency = UnsupportedCurrency };

        // Act
        await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        await _customerRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsValidationErrorWithFieldErrors_WhenCommandIsInvalid()
    {
        // Arrange
        var command = ValidCommand with { Currency = string.Empty };

        // Act
        var result = await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error!.Type);
        Assert.Contains(nameof(CreateInvoiceCommand.Currency), result.Error!.FieldErrors);
    }

    [Fact]
    public async Task HandleAsync_DoesNotLookUpCustomer_WhenCommandIsInvalid()
    {
        // Arrange
        var command = ValidCommand with { Currency = string.Empty };

        // Act
        await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        await _customerRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }
}
