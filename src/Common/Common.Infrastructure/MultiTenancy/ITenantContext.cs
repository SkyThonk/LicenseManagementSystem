namespace Common.Infrastructure.MultiTenancy;

/// <summary>
/// Interface for accessing tenant context information.
/// The tenant ID is resolved from JWT claims for each HTTP request.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// The current tenant's unique identifier, resolved from JWT claims.
    /// </summary>
    Guid TenantId { get; }

    /// <summary>
    /// The tenant's database connection string.
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// Whether a valid tenant context is available.
    /// </summary>
    bool IsAvailable { get; }
}
