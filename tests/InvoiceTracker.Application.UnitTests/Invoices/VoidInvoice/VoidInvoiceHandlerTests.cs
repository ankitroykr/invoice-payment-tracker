using InvoiceTracker.Application.Invoices.VoidInvoice;
using InvoiceTracker.Domain.Common;
using InvoiceTracker.Domain.Invoices;

using NSubstitute;

namespace InvoiceTracker.Application.UnitTests.Invoices.VoidInvoice;

public sealed class VoidInvoiceHandlerTests
{
    private const int InvoiceId = 1;

    private const int InvalidInvoiceId = 0;

    private const int CustomerId = 1;

    private const string Currency = "GBP";

    private const int MinorUnits = 2;

    private const string LineDescription = "Senior Engineer Onsite";

    private const decimal LineQuantity = 7.5m;

    private const decimal LineUnitPrice = 40m;

    private readonly IInvoiceRepository _invoiceRepository = Substitute.For<IInvoiceRepository>();

    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly VoidInvoiceCommandValidator _voidInvoiceCommandValidator = new();

    private readonly VoidInvoiceHandler _handler;

    public VoidInvoiceHandlerTests()
    {
        _handler = new VoidInvoiceHandler(_voidInvoiceCommandValidator, _invoiceRepository, _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenInvoiceDoesNotExist()
    {
        // Arrange
        _invoiceRepository.GetByIdAsync(InvoiceId, Arg.Any<CancellationToken>())
            .Returns((Invoice?)null);

        // Act
        var result =
            await _handler.HandleAsync(new VoidInvoiceCommand(InvoiceId), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(InvoiceErrors.NotFound, result.Error);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_VoidsTheInvoice_WhenItIsIssued()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        _invoiceRepository.GetByIdAsync(InvoiceId, Arg.Any<CancellationToken>())
            .Returns(invoice);

        // Act
        var result =
            await _handler.HandleAsync(new VoidInvoiceCommand(InvoiceId), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(InvoiceStatus.Void, invoice.Status);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotIssued_WhenInvoiceIsStillDraft()
    {
        // Arrange
        var invoice = CreateDraftInvoice();

        _invoiceRepository.GetByIdAsync(InvoiceId, Arg.Any<CancellationToken>())
            .Returns(invoice);

        // Act
        var result =
            await _handler.HandleAsync(new VoidInvoiceCommand(InvoiceId), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(InvoiceErrors.NotIssued, result.Error);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotIssued_WhenInvoiceIsAlreadyVoid()
    {
        // Arrange
        var invoice = CreateVoidInvoice();

        _invoiceRepository.GetByIdAsync(InvoiceId, Arg.Any<CancellationToken>())
            .Returns(invoice);

        // Act
        var result =
            await _handler.HandleAsync(new VoidInvoiceCommand(InvoiceId), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(InvoiceErrors.NotIssued, result.Error);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsValidationError_WhenInvoiceIdIsNotPositive()
    {
        // Arrange
        var command = new VoidInvoiceCommand(InvalidInvoiceId);

        // Act
        var result = await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        await _invoiceRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static Invoice CreateDraftInvoice()
    {
        var invoice = Invoice.Create(CustomerId, Currency, MinorUnits);
        _ = invoice.AddLine(LineDescription, LineQuantity, new Money(LineUnitPrice, Currency));
        return invoice;
    }

    private static Invoice CreateIssuedInvoice()
    {
        var invoice = CreateDraftInvoice();
        _ = invoice.Issue(1, new DateOnly(2026, 11, 17), new DateOnly(2026, 12, 17));
        return invoice;
    }

    private static Invoice CreateVoidInvoice()
    {
        var invoice = CreateIssuedInvoice();
        _ = invoice.Void();
        return invoice;
    }
}
