using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Domain.Invoices;

/// <summary>
/// Loads and adds invoices. Loading an invoice also loads its lines.
/// </summary>
public interface IInvoiceRepository : IRepository<Invoice>
{
}
