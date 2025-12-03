using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Licenses.UpdateLicenseStatus;

/// <summary>
/// Request to update license status
/// </summary>
public record UpdateLicenseStatusRequest(
    [Required]
    Guid Id,

    [Required]
    string NewStatus,

    DateTime? ExpiryDate = null
);
