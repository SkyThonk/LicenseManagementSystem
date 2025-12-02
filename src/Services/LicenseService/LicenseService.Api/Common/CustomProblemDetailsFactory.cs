using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LicenseService.Api;

public class CustomProblemDetailsFactory : ProblemDetailsFactory
{
    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode ?? 500,
            Title = title ?? "An error occurred",
            Type = type,
            Detail = detail,
            Instance = instance ?? httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        return problemDetails;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        var validationProblemDetails = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode ?? 400,
            Title = title ?? "One or more validation errors occurred",
            Type = type,
            Detail = detail,
            Instance = instance ?? httpContext.Request.Path
        };

        validationProblemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        return validationProblemDetails;
    }
}
