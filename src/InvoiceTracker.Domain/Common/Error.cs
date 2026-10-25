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
    /// Creates an error with the given code and message.
    /// </summary>
    /// <param name="code">The stable code that identifies the kind of error.</param>
    /// <param name="message">The message for people to read.</param>
    /// <exception cref="ArgumentNullException">The code or message is null.</exception>
    /// <exception cref="ArgumentException">The code or message is empty or whitespace.</exception>
    public Error(string code, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Code = code;
        Message = message;
    }
}
