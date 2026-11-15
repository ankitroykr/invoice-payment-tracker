using FluentValidation.TestHelper;

using InvoiceTracker.Application.Invoices.IssueInvoice;

using Microsoft.Extensions.Time.Testing;

namespace InvoiceTracker.Application.UnitTests.Invoices.IssueInvoice;

public sealed class IssueInvoiceCommandValidatorTests
{
    private const int InvalidInvoiceId = 0;

    private const int ValidInvoiceId = 1;

    private readonly FakeTimeProvider _fakeTimeProvider = new(new DateTimeOffset(2026, 11, 15, 0, 0, 0, TimeSpan.Zero));

    private readonly IssueInvoiceCommandValidator _validator;

    public IssueInvoiceCommandValidatorTests()
    {
        _validator = new IssueInvoiceCommandValidator(_fakeTimeProvider);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Validate_HasNoErrors_WhenCommandIsValid(int days)
    {
        // Arrange
        var command = new IssueInvoiceCommand(ValidInvoiceId,
            DateOnly.FromDateTime(_fakeTimeProvider.GetUtcNow().AddDays(days).Date));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_HasInvoiceIdError_WhenInvoiceIdIsNotPositive()
    {
        // Arrange
        var command =
            new IssueInvoiceCommand(InvalidInvoiceId, DateOnly.FromDateTime(_fakeTimeProvider.GetUtcNow().Date));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(i => i.InvoiceId);
    }

    [Fact]
    public void Validate_HasDueDateError_WhenDueDateIsBeforeToday()
    {
        // Arrange
        var command = new IssueInvoiceCommand(ValidInvoiceId,
            DateOnly.FromDateTime(_fakeTimeProvider.GetUtcNow().AddDays(-1).Date));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(i => i.DueDate);
    }
}
