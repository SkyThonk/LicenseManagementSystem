using Common.Application.Authentication.Dto;

namespace Common.Application.Interfaces.Authentication;

public interface IUserContext
{
    Guid UserId { get; }
    string? Email { get; }
    IReadOnlyList<UserRoleContext> Roles { get; }
    Guid? TenantId { get; }
    bool IsAuthenticated { get; }
    bool HasRoleCode(string roleCode);
}
