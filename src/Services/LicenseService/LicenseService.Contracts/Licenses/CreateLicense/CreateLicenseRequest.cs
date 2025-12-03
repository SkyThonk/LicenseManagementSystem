using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Licenses.CreateLicense;

/// <summary>
/// Request to create a new license application
/// </summary>
public record CreateLicenseRequest(
    [Required]
    Guid TenantId,

    [Required]
    Guid ApplicantId,

    [Required]
    Guid LicenseTypeId
);
