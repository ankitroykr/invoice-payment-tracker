namespace InvoiceTracker.Domain.Customers;

/// <summary>
/// A customer who can be sent invoices. Create one with <see cref="Create"/>.
/// </summary>
public sealed class Customer
{
    /// <summary>
    /// The customer's identifier. The database assigns it when the customer is saved, so it is 0 on a new customer.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// The customer's name, which is never empty or whitespace.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The customer's email address, which is never empty or whitespace.
    /// </summary>
    public string Email { get; private set; }

    private Customer(string name, string email)
    {
        Name = name;
        Email = email;
    }

    /// <summary>
    /// Creates a customer and checks the name and email are present.
    /// </summary>
    /// <param name="name">The customer's name.</param>
    /// <param name="email">The customer's email address. Only checked for being present, not for being a valid address.</param>
    /// <returns>A new <see cref="Customer"/> with the given name and email.</returns>
    /// <exception cref="ArgumentNullException">The name or email is null.</exception>
    /// <exception cref="ArgumentException">The name or email is empty or whitespace.</exception>
    public static Customer Create(string name, string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return new Customer(name, email);
    }
}
