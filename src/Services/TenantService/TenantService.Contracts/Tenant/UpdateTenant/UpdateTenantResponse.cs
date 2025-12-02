namespace TenantService.Contracts.Tenant.UpdateTenant;

/// <summary>
/// Response after updating a government agency
/// </summary>
public record UpdateTenantResponse(
    Guid Id,
    string Name,
    string AgencyCode,
    bool IsActive,
    DateTime? UpdatedAt
);

