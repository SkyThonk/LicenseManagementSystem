namespace Common.Domain.Events;

/// <summary>
/// Integration event raised when a new tenant (government agency) is created.
/// This event is published to the message broker and consumed by all services
/// to provision tenant-specific resources (schemas, tables, configurations, etc.)
/// </summary>
public record TenantCreatedEvent(
    Guid TenantId,
    string Name,
    string AgencyCode,
    string Email,
    DateTime CreatedAt
);
