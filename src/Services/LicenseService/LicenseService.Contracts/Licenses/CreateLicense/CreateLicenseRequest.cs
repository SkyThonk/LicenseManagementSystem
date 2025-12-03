using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Licenses.CreateLicense;

/// <summary>
/// Request to create a new license application.
/// Each tenant has their own isolated database, so no TenantId is required.
/// </summary>
public record CreateLicenseRequest(
    [Required(ErrorMessage = "Applicant ID is required")]
    Guid ApplicantId,

    [Required(ErrorMessage = "License type ID is required")]
    Guid LicenseTypeId
);
