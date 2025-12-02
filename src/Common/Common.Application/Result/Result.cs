namespace Common.Application.Result;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public ErrorBase? Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(ErrorBase error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(ErrorBase error) => new(error);
}
