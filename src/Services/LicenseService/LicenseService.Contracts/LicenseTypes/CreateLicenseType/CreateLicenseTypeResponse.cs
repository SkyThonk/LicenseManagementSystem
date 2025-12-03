namespace LicenseService.Contracts.LicenseTypes.CreateLicenseType;

/// <summary>
/// Response after creating a new license type
/// </summary>
public record CreateLicenseTypeResponse(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Description,
    decimal FeeAmount,
    DateTime CreatedAt
);
