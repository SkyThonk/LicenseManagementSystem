namespace TenantService.Contracts.Authentication;

public record RequestPasswordResetResponse(
    bool Success,
    string Message
);
