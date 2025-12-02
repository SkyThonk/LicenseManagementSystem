namespace TenantService.Contracts.User.GetUserList;

public record GetUserListRequest(
    int Page = 1,
    int PageSize = 10,
    Guid? TenantId = null,
    string? Role = null,
    bool? IsActive = null
);
