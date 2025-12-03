namespace LicenseService.Contracts.LicenseTypes.GetLicenseType;

/// <summary>
/// License type details DTO.
/// Each tenant has their own isolated database, so no TenantId is included.
/// </summary>
public record LicenseTypeDto(
    Guid Id,
    string Name,
    string? Description,
    decimal FeeAmount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
