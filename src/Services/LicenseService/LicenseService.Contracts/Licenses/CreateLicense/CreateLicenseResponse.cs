namespace LicenseService.Contracts.Licenses.CreateLicense;

/// <summary>
/// Response after creating a new license application.
/// Each tenant has their own isolated database, so no TenantId is included.
/// </summary>
public record CreateLicenseResponse(
    Guid Id,
    Guid ApplicantId,
    Guid LicenseTypeId,
    string Status,
    DateTime SubmittedAt,
    DateTime CreatedAt
);
