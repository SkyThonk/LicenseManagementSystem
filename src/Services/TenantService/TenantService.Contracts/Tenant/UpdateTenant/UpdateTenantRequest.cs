using System.Text.Json.Serialization;

namespace TenantService.Contracts.Tenant.UpdateTenant;

/// <summary>
/// Request to update a government agency (tenant)
/// </summary>
public record UpdateTenantRequest(
    [property: JsonIgnore]
    Guid Id,
    string? Name,
    string? Description,
    string? Email,
    string? Logo,
    string? AddressLine,
    string? City,
    string? State,
    string? PostalCode,
    string? CountryCode,
    string? PhoneNumber
);

