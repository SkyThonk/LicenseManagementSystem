using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Renewals.GetRenewal;

/// <summary>
/// Request to get a renewal by ID
/// </summary>
public record GetRenewalRequest(
    [Required]
    Guid Id
);
