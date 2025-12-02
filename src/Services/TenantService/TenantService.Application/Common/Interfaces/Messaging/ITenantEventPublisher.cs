using Common.Domain.Events;

namespace TenantService.Application.Common.Interfaces.Messaging;

/// <summary>
/// Interface for publishing tenant lifecycle events to message broker.
/// </summary>
public interface ITenantEventPublisher
{
    /// <summary>
    /// Publishes a TenantCreatedEvent to notify other services.
    /// </summary>
    Task PublishTenantCreatedAsync(TenantCreatedEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a TenantUpdatedEvent to notify other services.
    /// </summary>
    Task PublishTenantUpdatedAsync(TenantUpdatedEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a TenantDeletedEvent to notify other services.
    /// </summary>
    Task PublishTenantDeletedAsync(TenantDeletedEvent @event, CancellationToken cancellationToken = default);
}
