namespace InvoiceTracker.Application.Customers;

/// <summary>
/// A customer as returned to API callers.
/// </summary>
/// <param name="Id">The customer's identifier, assigned by the database.</param>
/// <param name="Name">The customer's name.</param>
/// <param name="Email">The customer's email address.</param>
public sealed record CustomerResponse(int Id, string Name, string Email);
