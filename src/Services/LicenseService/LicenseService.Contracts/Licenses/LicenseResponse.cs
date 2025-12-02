namespace LicenseService.Contracts.Licenses;

public record LicenseResponse(
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
