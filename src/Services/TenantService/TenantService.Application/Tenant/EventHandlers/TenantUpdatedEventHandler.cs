using Common.Domain.Events;
using TenantService.Application.Common.Interfaces.Messaging;
using Microsoft.Extensions.Logging;

namespace TenantService.Application.Tenant.EventHandlers;

/// <summary>
/// Wolverine handler for TenantUpdatedEvent domain event.
/// Publishes the event to Kafka for other services to consume.
/// </summary>
public class TenantUpdatedEventHandler
{
    private readonly ITenantEventPublisher _eventPublisher;
    private readonly ILogger<TenantUpdatedEventHandler> _logger;

    public TenantUpdatedEventHandler(
        ITenantEventPublisher eventPublisher,
        ILogger<TenantUpdatedEventHandler> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    /// <summary>
    /// Handles the TenantUpdatedEvent by publishing it to Kafka.
    /// This is called by Wolverine when the domain event is dispatched.
    /// </summary>
    public async Task Handle(TenantUpdatedEvent @event, CancellationToken ct)
    {
        _logger.LogInformation(
            "Handling TenantUpdatedEvent for tenant {TenantId}",
            @event.TenantId);

        // Publish to Kafka for other services to consume
        await _eventPublisher.PublishTenantUpdatedAsync(@event, ct);

        _logger.LogInformation(
            "Successfully published TenantUpdatedEvent to message broker for tenant {TenantId}",
            @event.TenantId);
    }
}
