namespace TenantService.Contracts.Authentication;

public record ConfirmPasswordResetResponse(
    bool Success,
    string Message
);
