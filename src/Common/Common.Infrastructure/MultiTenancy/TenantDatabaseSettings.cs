namespace Common.Infrastructure.MultiTenancy;

/// <summary>
/// Configuration for tenant database connection string patterns.
/// </summary>
public class TenantDatabaseSettings
{
    public const string SectionName = "TenantDatabase";

    /// <summary>
    /// The central database connection string for Wolverine outbox pattern.
    /// This database is shared and handles message persistence.
    /// </summary>
    public string CentralConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// The connection string template for tenant databases.
    /// Use {TenantId} as placeholder for the tenant identifier.
    /// Example: "Host=localhost;Database=tenant_{TenantId};Username=postgres;Password=postgres"
    /// </summary>
    public string TenantConnectionStringTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Whether to use separate databases per tenant.
    /// If false, uses a single shared database with TenantId column filtering.
    /// </summary>
    public bool UseSeparateDatabases { get; set; } = true;

    /// <summary>
    /// The service prefix for tenant database names.
    /// Database name format: {ServicePrefix}_{TenantId}
    /// Example: "license" results in database name "license_abc123..."
    /// </summary>
    public string ServicePrefix { get; set; } = "service";
}
