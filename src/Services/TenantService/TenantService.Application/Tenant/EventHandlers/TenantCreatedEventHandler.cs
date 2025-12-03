using Common.Domain.Events;
using TenantService.Application.Common.Interfaces.Messaging;
using Microsoft.Extensions.Logging;

namespace TenantService.Application.Tenant.EventHandlers;

/// <summary>
/// Wolverine handler for TenantCreatedEvent domain event.
/// Publishes the event to Kafka for other services to consume.
/// </summary>
public class TenantCreatedEventHandler
{
    private readonly ITenantEventPublisher _eventPublisher;
    private readonly ILogger<TenantCreatedEventHandler> _logger;

    public TenantCreatedEventHandler(
        ITenantEventPublisher eventPublisher,
        ILogger<TenantCreatedEventHandler> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    /// <summary>
    /// Handles the TenantCreatedEvent by publishing it to Kafka.
    /// This is called by Wolverine when the domain event is dispatched.
    /// </summary>
    public async Task Handle(TenantCreatedEvent @event, CancellationToken ct)
    {
        _logger.LogInformation(
            "Handling TenantCreatedEvent for tenant {TenantId} ({AgencyCode})",
            @event.TenantId,
            @event.AgencyCode);

        // Publish to Kafka for other services to consume
        await _eventPublisher.PublishTenantCreatedAsync(@event, ct);

        _logger.LogInformation(
            "Successfully published TenantCreatedEvent to message broker for tenant {TenantId}",
            @event.TenantId);
    }
}
