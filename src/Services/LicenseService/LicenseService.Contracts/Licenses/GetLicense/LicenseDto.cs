namespace LicenseService.Contracts.Licenses.GetLicense;

/// <summary>
/// License details DTO.
/// Each tenant has their own isolated database, so no TenantId is included.
/// </summary>
public record LicenseDto(
    Guid Id,
    Guid ApplicantId,
    Guid LicenseTypeId,
    string? LicenseTypeName,
    string Status,
    DateTime SubmittedAt,
    DateTime? ApprovedAt,
    DateTime? ExpiryDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
