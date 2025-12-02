using System.Security.Claims;
using Common.Application.Authentication.Dto;

namespace Common.Application.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, string userName, IEnumerable<UserRoleContext> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal ValidateAndDecryptToken(string token);
}
