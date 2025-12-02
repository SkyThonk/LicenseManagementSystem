using Common.Domain.Events;

namespace Common.Infrastructure.Messaging;

/// <summary>
/// Interface for handling tenant lifecycle events.
/// Each service implements this to provision/deprovision tenant-specific resources.
/// </summary>
public interface ITenantEventHandler
{
    /// <summary>
    /// Handles tenant creation - provisions tenant-specific database schema/tables.
    /// </summary>
    Task HandleTenantCreatedAsync(TenantCreatedEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles tenant update - updates tenant metadata cache if needed.
    /// </summary>
    Task HandleTenantUpdatedAsync(TenantUpdatedEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles tenant deletion - cleans up or deactivates tenant resources.
    /// </summary>
    Task HandleTenantDeletedAsync(TenantDeletedEvent @event, CancellationToken cancellationToken = default);
}
