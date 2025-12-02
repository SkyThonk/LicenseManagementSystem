namespace TenantService.Contracts.Authentication;

public record LoginResponse(
    Guid EmployeeId,
    string Name,
    string Role,
    string Token
);

