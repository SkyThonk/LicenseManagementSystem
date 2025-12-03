using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.LicenseTypes.UpdateLicenseType;

/// <summary>
/// Request to update a license type
/// </summary>
public record UpdateLicenseTypeRequest(
    Guid Id,

    [Required]
    [MaxLength(200)]
    string Name,

    [MaxLength(500)]
    string? Description,

    [Required]
    [Range(0, double.MaxValue)]
    decimal FeeAmount
);
