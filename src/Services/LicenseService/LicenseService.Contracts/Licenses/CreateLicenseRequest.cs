namespace LicenseService.Contracts.Licenses;

public record CreateLicenseRequest(
    Guid TenantId,
    Guid ApplicantId,
    Guid LicenseTypeId
);
