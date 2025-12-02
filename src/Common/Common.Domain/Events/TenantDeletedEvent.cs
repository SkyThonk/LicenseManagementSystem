namespace Common.Domain.Events;

/// <summary>
/// Integration event raised when a tenant is deleted (soft delete).
/// Other services may subscribe to deactivate tenant-specific resources.
/// </summary>
public record TenantDeletedEvent(
    Guid TenantId,
    DateTime DeletedAt
);
