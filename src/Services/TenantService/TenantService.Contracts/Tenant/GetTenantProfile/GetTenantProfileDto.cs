using TenantService.Contracts.Common.Dto;

namespace TenantService.Contracts.Tenant.GetTenantProfile;

/// <summary>
/// DTO for government agency (tenant) profile
/// </summary>
public record TenantProfileDto(
    Guid Id,
    string Name,
    string AgencyCode,
    string? Description,
    string Email,
    string? Logo,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    AddressDto? Address,
    PhoneDto? Phone
);
