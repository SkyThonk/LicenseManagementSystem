using System.ComponentModel.DataAnnotations;

namespace TenantService.Contracts.Tenant.RegisterTenant;

/// <summary>
/// Request to register a new government agency (tenant) in the license management system
/// </summary>
public record RegisterTenantRequest(
    [Required]
    [MaxLength(200)]
    string Name,

    [Required]
    [MaxLength(50)]
    string AgencyCode,

    [MaxLength(500)]
    string? Description,

    [Required]
    [MaxLength(255)]
    string AddressLine,

    [Required]
    [MaxLength(100)]
    string City,

    [Required]
    [MaxLength(100)]
    string State,

    [Required]
    [MaxLength(20)]
    string PostalCode,

    [Required]
    [MaxLength(5)]
    string CountryCode,

    [Required]
    [MaxLength(15)]
    string PhoneNumber,

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    string Email,

    [Required]
    [MinLength(6)]
    [MaxLength(20)]
    string Password,

    string? Logo
);

