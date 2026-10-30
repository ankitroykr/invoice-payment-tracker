using System.Collections.ObjectModel;

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

    /// <summary>
    /// Gets a collection of field-specific errors, where the keys represent the field names
    /// and the values are arrays of associated error messages.
    /// Empty unless the error was created with field errors.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> FieldErrors { get; }

    private Error(string code, string message, ErrorType type, IReadOnlyDictionary<string, string[]> fieldErrors)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Code = code;
        Message = message;
        Type = type;
        FieldErrors = fieldErrors;
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
        return new Error(code, message, ErrorType.Validation, ReadOnlyDictionary<string, string[]>.Empty);
    }

    /// <summary>
    /// Creates an error for missing or malformed input, with the messages for each invalid field.
    /// </summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The readable error message.</param>
    /// <param name="fieldErrors">The error messages for each invalid field.</param>
    /// <exception cref="ArgumentNullException">The code, message, or fieldErrors are null.</exception>
    /// <exception cref="ArgumentException">The code or message is empty or whitespace, or fieldErrors is empty.</exception>
    public static Error Validation(string code, string message, IReadOnlyDictionary<string, string[]> fieldErrors)
    {
        ArgumentNullException.ThrowIfNull(fieldErrors);

        if (fieldErrors.Count == 0)
        {
            throw new ArgumentException("At least one field error must be provided.", nameof(fieldErrors));
        }

        return new Error(code, message, ErrorType.Validation, fieldErrors);
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
        return new Error(code, message, ErrorType.NotFound, ReadOnlyDictionary<string, string[]>.Empty);
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
        return new Error(code, message, ErrorType.Conflict, ReadOnlyDictionary<string, string[]>.Empty);
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
        return new Error(code, message, ErrorType.BusinessRule, ReadOnlyDictionary<string, string[]>.Empty);
    }
}
