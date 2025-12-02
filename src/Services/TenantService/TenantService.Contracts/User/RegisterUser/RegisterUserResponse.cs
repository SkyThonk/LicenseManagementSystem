namespace TenantService.Contracts.User.RegisterUser;

public record RegisterUserResponse(
    Guid UserId,
    string Email,
    string Role,
    DateTime CreatedAt
);
