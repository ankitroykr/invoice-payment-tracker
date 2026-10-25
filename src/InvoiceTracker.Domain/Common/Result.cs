namespace InvoiceTracker.Domain.Common;

/// <summary>
/// The outcome of a business action: success, or failure with an <see cref="Common.Error"/>.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets whether the action succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets whether the action failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the reason for the failure, or null on success.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Creates a result. Use <see cref="Success()"/> or <see cref="Failure(Common.Error)"/> instead.
    /// </summary>
    /// <param name="isSuccess">Whether the action succeeded.</param>
    /// <param name="error">The reason for the failure, or null on success.</param>
    /// <exception cref="InvalidOperationException">A success has an error, or a failure has none.</exception>
    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error is not null)
        {
            throw new InvalidOperationException("Cannot create a success result with an error.");
        }

        if (!isSuccess && error is null)
        {
            throw new InvalidOperationException("Cannot create a failure result without an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result with no associated error.
    /// </summary>
    /// <returns>A successful <see cref="Result"/> instance.</returns>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error representing the failure reason.</param>
    /// <returns>A failed <see cref="Result"/> instance containing the provided error.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result(false, error);
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value indicating the successful result.</param>
    /// <returns>A successful <see cref="Result{TValue}"/> instance containing the provided value.</returns>
    public static Result<TValue> Success<TValue>(TValue value) => new(true, null, value);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error associated with the failed result.</param>
    /// <typeparam name="TValue">The type of the value that would have been returned in a successful result.</typeparam>
    /// <returns>A failed <see cref="Result{TValue}"/> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static Result<TValue> Failure<TValue>(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result<TValue>(false, error, default!);
    }
}
