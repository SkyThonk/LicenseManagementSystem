namespace LicenseService.Contracts.LicenseTypes;

public record CreateLicenseTypeRequest(
    Guid TenantId,
    string Name,
    string? Description,
    decimal FeeAmount
);
