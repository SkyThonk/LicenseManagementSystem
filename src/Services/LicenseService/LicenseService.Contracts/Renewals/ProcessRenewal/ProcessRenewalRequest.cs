using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LicenseService.Contracts.Renewals.ProcessRenewal;

/// <summary>
/// Request to process a renewal (approve/reject)
/// </summary>
public record ProcessRenewalRequest(
    [property: JsonIgnore]
    Guid Id,

    [Required(ErrorMessage = "Approval decision is required")]
    bool Approve
);
