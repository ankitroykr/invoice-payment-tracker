namespace InvoiceTracker.Domain.Common;

/// <summary>
/// The reason a business action was refused, with a stable code and a readable message.
/// </summary>
public sealed record Error
{
    /// <summary>
    /// Gets the stable code that identifies the kind of error, for example, Invoice.NotDraft.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the descriptive message that provides details about the error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the classification of the error, indicating its general category.
    /// </summary>
    public ErrorType Type { get; }

    private Error(string code, string message, ErrorType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Code = code;
        Message = message;
        Type = type;
    }

    /// <summary>
    /// Creates an error for missing or malformed input.
    /// </summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The readable error message.</param>
    /// <exception cref="ArgumentNullException">The code or message is null.</exception>
    /// <exception cref="ArgumentException">The code or message is null, empty, or whitespace.</exception>
    public static Error Validation(string code, string message)
    {
        return new Error(code, message, ErrorType.Validation);
    }

    /// <summary>
    /// Creates an error for an item that does not exist.
    /// </summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The readable error message.</param>
    /// <exception cref="ArgumentNullException">The code or message is null.</exception>
    /// <exception cref="ArgumentException">The code or message is null, empty, or whitespace.</exception>
    public static Error NotFound(string code, string message)
    {
        return new Error(code, message, ErrorType.NotFound);
    }

    /// <summary>
    /// Creates an error for a clash with existing data.
    /// </summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The readable error message.</param>
    /// <exception cref="ArgumentNullException">The code or message is null.</exception>
    /// <exception cref="ArgumentException">The code or message is null, empty, or whitespace.</exception>
    public static Error Conflict(string code, string message)
    {
        return new Error(code, message, ErrorType.Conflict);
    }

    /// <summary>
    /// Creates an error for an action a business rule does not allow.
    /// </summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The readable error message.</param>
    /// <exception cref="ArgumentNullException">The code or message is null.</exception>
    /// <exception cref="ArgumentException">The code or message is null, empty, or whitespace.</exception>
    public static Error BusinessRule(string code, string message)
    {
        return new Error(code, message, ErrorType.BusinessRule);
    }
}
