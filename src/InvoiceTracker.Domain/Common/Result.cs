namespace InvoiceTracker.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error? Error { get; }

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

    public static Result Success() => new(true, null);

    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result(false, error);
    }
}
