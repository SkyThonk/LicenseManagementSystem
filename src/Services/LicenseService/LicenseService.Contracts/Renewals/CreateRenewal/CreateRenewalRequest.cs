using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Renewals.CreateRenewal;

/// <summary>
/// Request to create a new renewal
/// </summary>
public record CreateRenewalRequest(
    [Required]
    Guid TenantId,

    [Required]
    Guid LicenseId,

    [Required]
    DateTime RenewalDate
);
