using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.LicenseTypes.DeleteLicenseType;

/// <summary>
/// Request to delete a license type
/// </summary>
public record DeleteLicenseTypeRequest(
    [Required]
    Guid Id
);
