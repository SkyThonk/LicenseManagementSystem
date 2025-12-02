namespace Common.Application.Result;

// Base class for all errors
public abstract record ErrorBase
{
    public string[] Messages { get; init; } = Array.Empty<string>();
    public Dictionary<string, string[]>? FieldErrors { get; init; }
}

// Validation error
public record ValidationError : ErrorBase
{
    public ValidationError(params string[] messages) => Messages = messages;
    public ValidationError(Dictionary<string, string[]> fieldErrors) => FieldErrors = fieldErrors;
}

// NotFound error
public record NotFoundError : ErrorBase
{
    public NotFoundError(params string[] messages) => Messages = messages;
}

// Conflict error
public record ConflictError : ErrorBase
{
    public ConflictError(params string[] messages) => Messages = messages;
}

// Unauthorized error
public record UnauthorizedError : ErrorBase
{
    public UnauthorizedError(params string[] messages) => Messages = messages;
}

// Unexpected error
public record UnexpectedError : ErrorBase
{
    public UnexpectedError(params string[] messages) => Messages = messages;
}
