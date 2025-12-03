using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Renewals.CreateRenewal;

/// <summary>
/// Request to create a new renewal.
/// Each tenant has their own isolated database, so no TenantId is required.
/// </summary>
public record CreateRenewalRequest(
    [Required]
    Guid LicenseId,

    [Required]
    DateTime RenewalDate
);
