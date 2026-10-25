namespace InvoiceTracker.Domain.Common;

/// <summary>
/// The outcome of a business action that also returns a value on success.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public sealed class Result<TValue> : Result
{
    private readonly TValue _value;

    /// <summary>
    /// Gets the value, only available on success.
    /// </summary>
    /// <exception cref="InvalidOperationException">The result is a failure.</exception>
    public TValue Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("Cannot get value from a failure result.");
            }

            return _value;
        }
    }

    internal Result(bool isSuccess, Error? error, TValue value) : base(isSuccess, error)
    {
        _value = value;
    }
}
