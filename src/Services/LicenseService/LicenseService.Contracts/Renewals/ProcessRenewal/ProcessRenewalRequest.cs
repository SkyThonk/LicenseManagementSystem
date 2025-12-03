using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Renewals.ProcessRenewal;

/// <summary>
/// Request to process a renewal (approve/reject)
/// </summary>
public record ProcessRenewalRequest(
    [Required]
    Guid Id,

    [Required]
    bool Approve
);
