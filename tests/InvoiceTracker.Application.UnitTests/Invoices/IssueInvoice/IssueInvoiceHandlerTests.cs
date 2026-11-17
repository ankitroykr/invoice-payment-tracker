using InvoiceTracker.Application.Invoices;
using InvoiceTracker.Application.Invoices.IssueInvoice;
using InvoiceTracker.Domain.Common;
using InvoiceTracker.Domain.Invoices;

using Microsoft.Extensions.Time.Testing;

using NSubstitute;

namespace InvoiceTracker.Application.UnitTests.Invoices.IssueInvoice;

public sealed class IssueInvoiceHandlerTests
{
    private const int InvoiceId = 1;

    private const int CustomerId = 1;

    private const string Currency = "GBP";

    private const int MinorUnits = 2;

    private const string LineDescription = "Senior Engineer Onsite";

    private const decimal LineQuantity = 7.5m;

    private const decimal LineUnitPrice = 40m;

    private readonly FakeTimeProvider _fakeTimeProvider = new(new DateTimeOffset(2026, 11, 14, 0, 0, 0, TimeSpan.Zero));

    private readonly IInvoiceRepository _invoiceRepository = Substitute.For<IInvoiceRepository>();

    private readonly IInvoiceNumberCounter _invoiceNumberCounter = Substitute.For<IInvoiceNumberCounter>();

    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly IssueInvoiceHandler _handler;

    public IssueInvoiceHandlerTests()
    {
        IssueInvoiceCommandValidator issueInvoiceCommandValidator = new(_fakeTimeProvider);
        _handler = new IssueInvoiceHandler(issueInvoiceCommandValidator, _invoiceRepository, _invoiceNumberCounter,
            _unitOfWork, _fakeTimeProvider);
    }

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenInvoiceDoesNotExist()
    {
        // Arrange
        _invoiceRepository.GetByIdAsync(InvoiceId, Arg.Any<CancellationToken>())
            .Returns((Invoice?)null);

        // Act
        var result = await _handler.HandleAsync(new IssueInvoiceCommand(InvoiceId, new DateOnly(2027, 01, 01)),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(InvoiceErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task HandleAsync_IssuesTheInvoice_WhenItIsADraftWithLines()
    {
        // Arrange
        var invoice = CreateDraftInvoice();

        _invoiceRepository.GetByIdAsync(InvoiceId, Arg.Any<CancellationToken>())
            .Returns(invoice);
        _invoiceNumberCounter.GetNextSequenceAsync(2026, Arg.Any<CancellationToken>())
            .Returns(5);

        // Act
        var result = await _handler.HandleAsync(new IssueInvoiceCommand(InvoiceId, new DateOnly(2026, 11, 14)),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
        Assert.Equal(new InvoiceNumber(2026, 5), invoice.Number);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_DoesNotIncreaseTheSequence_WhenInvoiceHasNoLines()
    {
        // Arrange
        var invoice = Invoice.Create(CustomerId, Currency, MinorUnits);

        _invoiceRepository.GetByIdAsync(InvoiceId, Arg.Any<CancellationToken>())
            .Returns(invoice);

        // Act
        var result = await _handler.HandleAsync(new IssueInvoiceCommand(InvoiceId, new DateOnly(2026, 11, 14)),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(InvoiceErrors.NoLines, result.Error);
        await _invoiceNumberCounter.DidNotReceive().GetNextSequenceAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_DoesNotIncreaseTheSequence_WhenInvoiceIsAlreadyIssued()
    {
        // Arrange
        var invoice = CreateIssuedInvoice();

        _invoiceRepository.GetByIdAsync(InvoiceId, Arg.Any<CancellationToken>())
            .Returns(invoice);

        // Act
        var result = await _handler.HandleAsync(new IssueInvoiceCommand(InvoiceId, new DateOnly(2026, 12, 14)),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(InvoiceErrors.NotDraft, result.Error);
        await _invoiceNumberCounter.DidNotReceive().GetNextSequenceAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsValidationError_WhenDueDateIsBeforeToday()
    {
        // Arrange
        var invoice = CreateDraftInvoice();

        _invoiceRepository.GetByIdAsync(InvoiceId, Arg.Any<CancellationToken>())
            .Returns(invoice);

        // Act
        var result = await _handler.HandleAsync(new IssueInvoiceCommand(InvoiceId, new DateOnly(2026, 11, 13)),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(ErrorType.Validation, result.Error?.Type);
        await _invoiceNumberCounter.DidNotReceive().GetNextSequenceAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
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
        _ = invoice.Issue(1, new DateOnly(2026, 11, 14), new DateOnly(2026, 12, 14));
        return invoice;
    }
}
