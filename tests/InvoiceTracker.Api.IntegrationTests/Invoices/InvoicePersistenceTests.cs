using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Domain.Common;
using InvoiceTracker.Domain.Customers;
using InvoiceTracker.Domain.Invoices;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceTracker.Api.IntegrationTests.Invoices;

public sealed class InvoicePersistenceTests(InvoiceTrackerApiFactory factory)
{
    private const string ValidCustomerName = "Test Customer";

    private const string ValidCustomerEmail = "test@example.com";

    private const string Currency = "GBP";

    // SQL Server error 547: a statement conflicted with a FOREIGN KEY or CHECK constraint.
    private const int ConstraintViolationErrorNumber = 547;

    private const string CurrencyForeignKeyName = "FK_Invoices_Currencies_Currency";

    // SQL Server error 2601: a duplicate key row in a unique index.
    private const int DuplicateKeyErrorNumber = 2601;

    private static readonly DateOnly IssueDate = new(2090, 1, 15);

    private static readonly DateOnly DueDate = new(2090, 2, 14);

    [Fact]
    public async Task GetByIdAsync_ReturnsInvoiceWithLinesAndNumber_WhenIssuedInvoiceIsSaved()
    {
        // Arrange
        const string description = "Senior Engineer Onsite";
        const decimal quantity = 7.5m;
        var unitPrice = new Money(37m, Currency);
        int customerId;
        int invoiceId;
        await using (var arrangeScope = factory.Services.CreateAsyncScope())
        {
            var invoiceRepository = arrangeScope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
            customerId = await AddCustomerAsync(arrangeScope.ServiceProvider);
            var invoice = Invoice.Create(customerId, Currency);
            invoice.AddLine(description, quantity, unitPrice);
            invoice.Issue(1, IssueDate, DueDate);
            invoiceRepository.Add(invoice);
            await invoiceRepository.UnitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken);
            invoiceId = invoice.Id;
        }

        await using var actScope = factory.Services.CreateAsyncScope();
        var actRepository = actScope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

        // Act
        var savedInvoice = await actRepository.GetByIdAsync(invoiceId, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(savedInvoice);
        Assert.Equal(customerId, savedInvoice.CustomerId);
        Assert.Equal("GBP", savedInvoice.Currency);
        Assert.Equal(new InvoiceNumber(2090, 1), savedInvoice.Number);
        Assert.Equal(InvoiceStatus.Issued, savedInvoice.Status);
        Assert.Equal(new DateOnly(2090, 1, 15), savedInvoice.IssueDate);
        Assert.Equal(new DateOnly(2090, 2, 14), savedInvoice.DueDate);
        var line = Assert.Single(savedInvoice.Lines);
        Assert.Equal(description, line.Description);
        Assert.Equal(quantity, line.Quantity);
        Assert.Equal(unitPrice, line.UnitPrice);
        Assert.Equal(new Money(277.5m, Currency), savedInvoice.Total);
    }

    [Fact]
    public async Task SaveChangesAsync_ThrowsDbUpdateException_WhenInvoiceNumberIsAlreadyUsed()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
        var customerId = await AddCustomerAsync(scope.ServiceProvider);
        var firstInvoice = Invoice.Create(customerId, Currency);
        firstInvoice.AddLine("Senior Engineer Remote", 1m, new Money(40m, Currency));
        firstInvoice.Issue(2, IssueDate, DueDate);
        invoiceRepository.Add(firstInvoice);
        await invoiceRepository.UnitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken);
        var secondInvoice = Invoice.Create(customerId, Currency);
        secondInvoice.AddLine("Senior Engineer Remote", 1m, new Money(35m, Currency));
        secondInvoice.Issue(2, IssueDate, DueDate);
        invoiceRepository.Add(secondInvoice);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() =>
            invoiceRepository.UnitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken));

        var sqlException = Assert.IsType<SqlException>(exception.InnerException);
        Assert.Equal(DuplicateKeyErrorNumber, sqlException.Number);
    }

    [Fact]
    public async Task SaveChangesAsync_SavesBoth_WhenTwoDraftsHaveNoNumber()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
        var customerId = await AddCustomerAsync(scope.ServiceProvider);
        var firstDraft = Invoice.Create(customerId, Currency);
        var secondDraft = Invoice.Create(customerId, Currency);
        invoiceRepository.Add(firstDraft);
        invoiceRepository.Add(secondDraft);

        // Act
        await invoiceRepository.UnitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(firstDraft.Id > 0);
        Assert.True(secondDraft.Id > 0);
    }

    [Fact]
    public async Task SaveChangesAsync_ThrowsDbUpdateException_WhenCurrencyIsNotSupported()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
        var customerId = await AddCustomerAsync(scope.ServiceProvider);
        invoiceRepository.Add(Invoice.Create(customerId, "JPY"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() =>
            invoiceRepository.UnitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken));

        var sqlException = Assert.IsType<SqlException>(exception.InnerException);
        Assert.Equal(ConstraintViolationErrorNumber, sqlException.Number);
        Assert.Contains(CurrencyForeignKeyName, sqlException.Message);
    }

    private static async Task<int> AddCustomerAsync(IServiceProvider serviceProvider)
    {
        var customerRepository = serviceProvider.GetRequiredService<ICustomerRepository>();
        var customer = Customer.Create(ValidCustomerName, ValidCustomerEmail);
        customerRepository.Add(customer);
        await customerRepository.UnitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken);
        return customer.Id;
    }
}
