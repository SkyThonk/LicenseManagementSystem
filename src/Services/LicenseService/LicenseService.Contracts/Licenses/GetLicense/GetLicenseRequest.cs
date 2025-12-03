using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Licenses.GetLicense;

/// <summary>
/// Request to get a license by ID
/// </summary>
public record GetLicenseRequest(
    [Required]
    Guid Id
);
