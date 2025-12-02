using Common.Application.Result;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Extensions;

public static class ResultExtensions
{
    // MVC Controllers version
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);

        var error = result.Error;
        if (error is null)
            return controller.Problem(statusCode: 500, detail: "Unknown error");

        // Field-specific validation errors
        if (error.FieldErrors is not null)
            return controller.BadRequest(new ValidationProblemDetails(error.FieldErrors));

        // Determine HTTP status based on error type
        int statusCode = error switch
        {
            ValidationError => 400,
            NotFoundError => 404,
            ConflictError => 409,
            UnauthorizedError => 401,
            UnexpectedError => 500,
            _ => 500
        };

        string? detail = error.Messages.Length > 0 ? string.Join(" | ", error.Messages) : null;

        return controller.Problem(statusCode: statusCode, detail: detail);
    }

    // Minimal API version
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result.Value);

        var error = result.Error;
        if (error is null)
            return Results.Problem(statusCode: 500, detail: "Unknown error");

        if (error.FieldErrors is not null)
            return Results.BadRequest(new ValidationProblemDetails(error.FieldErrors));

        int statusCode = error switch
        {
            ValidationError => 400,
            NotFoundError => 404,
            ConflictError => 409,
            UnauthorizedError => 401,
            UnexpectedError => 500,
            _ => 500
        };

        string? detail = error.Messages.Length > 0 ? string.Join(" | ", error.Messages) : null;

        return Results.Problem(statusCode: statusCode, detail: detail);
    }
}

