namespace LicenseService.Contracts.Licenses.CreateLicense;

/// <summary>
/// Response after creating a new license application
/// </summary>
public record CreateLicenseResponse(
    Guid Id,
    Guid TenantId,
    Guid ApplicantId,
    Guid LicenseTypeId,
    string Status,
    DateTime SubmittedAt,
    DateTime CreatedAt
);
