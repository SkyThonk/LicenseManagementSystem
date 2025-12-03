namespace LicenseService.Contracts.Licenses.GetLicense;

/// <summary>
/// License details DTO
/// </summary>
public record LicenseDto(
    Guid Id,
    Guid TenantId,
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
