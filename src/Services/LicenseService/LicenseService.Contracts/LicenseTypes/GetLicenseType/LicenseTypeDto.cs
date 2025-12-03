namespace LicenseService.Contracts.LicenseTypes.GetLicenseType;

/// <summary>
/// License type details DTO
/// </summary>
public record LicenseTypeDto(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Description,
    decimal FeeAmount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
