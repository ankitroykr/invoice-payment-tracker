namespace InvoiceTracker.Api.Security;

/// <summary>
/// The names of the API's authorization policies, for use in <c>[Authorize(Policy = ...)]</c>. Each policy
/// checks the caller's <see cref="AppRoles">app roles</see>.
/// </summary>
public static class Policies
{
    /// <summary>
    /// Lets the caller read customers and invoices. Needs the <see cref="AppRoles.Read"/> or
    /// <see cref="AppRoles.ReadWrite"/> role.
    /// </summary>
    public const string CanRead = "CanRead";

    /// <summary>
    /// Lets the caller create and change customers and invoices. Needs the <see cref="AppRoles.ReadWrite"/> role.
    /// </summary>
    public const string CanWrite = "CanWrite";
}
