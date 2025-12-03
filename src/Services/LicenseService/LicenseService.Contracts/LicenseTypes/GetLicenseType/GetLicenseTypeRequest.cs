using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.LicenseTypes.GetLicenseType;

/// <summary>
/// Request to get a license type by ID
/// </summary>
public record GetLicenseTypeRequest(
    [Required]
    Guid Id
);
