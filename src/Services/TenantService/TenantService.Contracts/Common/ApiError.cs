namespace TenantService.Contracts.Common;

public record ApiError(int StatusCode, string Message)
{
    public static ApiError BadRequest(string message)
        => new(400, message);

    public static ApiError Unauthorized(string message)
        => new(401, message);

    public static ApiError Forbidden(string message)
        => new(403, message);

    public static ApiError NotFound(string message)
        => new(404, message);

    public static ApiError Conflict(string message)
        => new(409, message);
    
    public static ApiError Validation(string message)
        => new(422, message);
    public static ApiError ServiceUnavailable(string message)
        => new(503, message);
        
   
}

