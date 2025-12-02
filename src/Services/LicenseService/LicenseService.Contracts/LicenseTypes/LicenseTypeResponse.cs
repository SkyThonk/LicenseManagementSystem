namespace LicenseService.Contracts.LicenseTypes;

public record LicenseTypeResponse(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Description,
    decimal FeeAmount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
