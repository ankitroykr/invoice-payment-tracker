namespace InvoiceTracker.Domain.Common;

public sealed class Result<TValue> : Result
{
    private readonly TValue _value;

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
