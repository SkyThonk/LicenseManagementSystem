using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LicenseService.Contracts.Licenses.UpdateLicenseStatus;

/// <summary>
/// Request to update license status
/// </summary>
public record UpdateLicenseStatusRequest(
    [property: JsonIgnore]
    Guid Id,

    [Required(ErrorMessage = "Status is required")]
    [RegularExpression("^(Approved|Rejected|Pending|Cancelled)$", ErrorMessage = "Invalid status value")]
    string NewStatus,

    DateTime? ExpiryDate = null
);
