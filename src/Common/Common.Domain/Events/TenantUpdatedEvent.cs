using Common.Domain.Abstractions;

namespace Common.Domain.Events;

/// <summary>
/// Integration event raised when a tenant is updated.
/// Other services may subscribe to update their tenant metadata cache.
/// </summary>
public record TenantUpdatedEvent(
    Guid TenantId,
    string Name,
    string AgencyCode,
    string Email,
    bool IsActive,
    DateTime UpdatedAt
) : IDomainEvent;
