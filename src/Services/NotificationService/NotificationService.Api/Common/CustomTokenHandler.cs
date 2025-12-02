using Common.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace NotificationService.Api.Common;

public class CustomTokenRequirement : IAuthorizationRequirement { }

public class CustomTokenHandler : AuthorizationHandler<CustomTokenRequirement>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public CustomTokenHandler(IJwtTokenGenerator jwtTokenGenerator) => _jwtTokenGenerator = jwtTokenGenerator;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomTokenRequirement requirement)
    {
        var httpContext = (context.Resource as DefaultHttpContext)!;
        var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        try
        {
            var claimsPrincipal = _jwtTokenGenerator.ValidateAndDecryptToken(token);
            if (claimsPrincipal != null)
            {
                context.Succeed(requirement);
            }
        }
        catch
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
