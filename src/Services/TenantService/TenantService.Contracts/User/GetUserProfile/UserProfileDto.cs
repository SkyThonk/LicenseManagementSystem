namespace TenantService.Contracts.User.GetUserProfile;

public record UserProfileDto(
    Guid UserId,
    string Email,
    string? FirstName,
    string? LastName,
    string Role,
    Guid TenantId,
    string TenantName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
