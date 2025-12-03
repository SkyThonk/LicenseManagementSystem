namespace LicenseService.Contracts.LicenseTypes.CreateLicenseType;

/// <summary>
/// Response after creating a new license type.
/// Each tenant has their own isolated database, so no TenantId is included.
/// </summary>
public record CreateLicenseTypeResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal FeeAmount,
    DateTime CreatedAt
);
