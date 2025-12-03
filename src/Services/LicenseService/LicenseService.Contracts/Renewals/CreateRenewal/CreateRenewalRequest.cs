using System.ComponentModel.DataAnnotations;

namespace LicenseService.Contracts.Renewals.CreateRenewal;

/// <summary>
/// Request to create a new renewal.
/// Each tenant has their own isolated database, so no TenantId is required.
/// </summary>
public record CreateRenewalRequest(
    [Required(ErrorMessage = "License ID is required")]
    Guid LicenseId,

    [Required(ErrorMessage = "Renewal date is required")]
    DateTime RenewalDate
);
