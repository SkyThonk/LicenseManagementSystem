namespace TenantService.Contracts.User.GetUserList;

public record UserListItemDto(
    Guid UserId,
    string Email,
    string? FirstName,
    string? LastName,
    string Role,
    Guid TenantId,
    bool IsActive,
    DateTime CreatedAt
);
