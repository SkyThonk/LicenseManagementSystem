using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LicenseService.Contracts.LicenseTypes.UpdateLicenseType;

/// <summary>
/// Request to update a license type
/// </summary>
public record UpdateLicenseTypeRequest(
    [property: JsonIgnore]
    Guid Id,

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    string Name,

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    string? Description,

    [Required(ErrorMessage = "Fee amount is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Fee amount must be non-negative")]
    decimal FeeAmount
);
