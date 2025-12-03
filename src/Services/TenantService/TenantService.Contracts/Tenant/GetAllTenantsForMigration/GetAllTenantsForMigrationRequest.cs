namespace TenantService.Contracts.Tenant.GetAllTenantsForMigration;

/// <summary>
/// Request to get all tenants for database migration (no authentication required)
/// </summary>
public record GetAllTenantsForMigrationRequest();

/// <summary>
/// Tenant data for migration purposes
/// </summary>
public record TenantMigrationDto(
    Guid Id,
    string Name,
    string? ConnectionString
);

/// <summary>
/// Response containing all tenants for migration
/// </summary>
public record GetAllTenantsForMigrationResponse(
    IEnumerable<TenantMigrationDto> Tenants
);
