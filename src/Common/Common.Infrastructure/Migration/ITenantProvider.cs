namespace Common.Infrastructure.Migration;

/// <summary>
/// Tenant information for multi-tenant database migration
/// </summary>
public record TenantInfo(
    Guid Id,
    string Name,
    string ConnectionString
);

/// <summary>
/// Interface to provide tenant information for migration
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// Get all active tenants for database migration
    /// </summary>
    Task<IEnumerable<TenantInfo>> GetAllTenantsAsync(CancellationToken ct = default);
}
