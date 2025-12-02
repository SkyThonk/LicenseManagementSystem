namespace TenantService.Contracts.User.GetUserList;

public record GetUserListResponse(
    List<UserListItemDto> Users,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
