using System.Security.Claims;
using System.Text.Json;
using Common.Application.Authentication.Dto;
using Common.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Http;

namespace Common.Infrastructure.Authentication;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid UserId =>
        Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : Guid.Empty;

    public string? Email =>
        User?.FindFirst(ClaimTypes.Email)?.Value;

    public IReadOnlyList<UserRoleContext> Roles
    {
        get
        {
            var rolesClaim = User?.FindFirst("roles");
            if (rolesClaim is null) return Array.Empty<UserRoleContext>();

            try
            {
                var roles = JsonSerializer.Deserialize<List<UserRoleContext>>(rolesClaim.Value);
                return roles ?? (IReadOnlyList<UserRoleContext>)Array.Empty<UserRoleContext>();
            }
            catch (JsonException)
            {
                return Array.Empty<UserRoleContext>();
            }
        }
    }
    
    public Guid? TenantId =>
        Guid.TryParse(User?.FindFirst("tenantId")?.Value, out var tenantId) ? tenantId : null;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    
    public bool HasRoleCode(string roleCode)
    {
        return Roles.Any(r => r.Code == roleCode);
    }
}
