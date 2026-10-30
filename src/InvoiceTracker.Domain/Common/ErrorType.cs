namespace InvoiceTracker.Domain.Common;

/// <summary>
/// The category of an <see cref="Error"/>, which tells callers why a business action was refused.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// The input was missing or malformed, for example, an empty name or a negative amount.
    /// </summary>
    Validation,

    /// <summary>
    /// The requested item does not exist.
    /// </summary>
    NotFound,

    /// <summary>
    /// The action clashes with the current state of existing data, for example, a duplicate email.
    /// </summary>
    Conflict,

    /// <summary>
    /// The input was valid, but a business rule does not allow the action, for example, editing an invoice that is no longer a draft.
    /// </summary>
    BusinessRule
}
