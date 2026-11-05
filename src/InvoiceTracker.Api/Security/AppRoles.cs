namespace InvoiceTracker.Api.Security;

/// <summary>
/// The app roles defined on the API's Entra ID app registration. Entra ID sends the caller's roles in the
/// <c>roles</c> claim of the access token.
/// </summary>
public static class AppRoles
{
    /// <summary>
    /// Can read customers and invoices.
    /// </summary>
    public const string Read = "InvoiceTracker.Read";

    /// <summary>
    /// Can read, create and change customers and invoices.
    /// </summary>
    public const string ReadWrite = "InvoiceTracker.ReadWrite";
}
