namespace TenantService.Contracts.Authentication;

public record LoginResponse(
    Guid UserId,
    string Email,
    string? FirstName,
    string? LastName,
    string Role,
    string Token
);

