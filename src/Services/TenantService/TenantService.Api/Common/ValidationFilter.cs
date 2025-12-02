using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TenantService.Api.Common;

/// <summary>
/// Global endpoint filter that validates request DTOs using DataAnnotations.
/// Returns a 400 ValidationProblem response when validation fails.
/// </summary>
public sealed class ValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (var arg in context.Arguments)
        {
            if (arg is null) continue;

            var validationContext = new ValidationContext(arg);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                arg,
                validationContext,
                validationResults,
                validateAllProperties: true
            );

            if (!isValid)
            {
                var errors = validationResults
                    .SelectMany(r => r.MemberNames.Select(m => new { Member = m, r.ErrorMessage }))
                    .GroupBy(x => x.Member)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage ?? string.Empty).ToArray()
                    );

                return Results.ValidationProblem(errors);
            }
        }

        return await next(context);
    }
}

