using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.LicenseTypes.CreateLicenseType;

/// <summary>
/// Request to create a new license type.
/// Each tenant has their own isolated database, so no TenantId is required.
/// </summary>
public record CreateLicenseTypeRequest(
    [Required]
    [MaxLength(200)]
    string Name,

    [MaxLength(500)]
    string? Description,

    [Required]
    [Range(0, double.MaxValue)]
    decimal FeeAmount
);
