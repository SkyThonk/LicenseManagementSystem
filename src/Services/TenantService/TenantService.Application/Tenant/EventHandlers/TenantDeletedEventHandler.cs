using Common.Domain.Events;
using TenantService.Application.Common.Interfaces.Messaging;
using Microsoft.Extensions.Logging;

namespace TenantService.Application.Tenant.EventHandlers;

/// <summary>
/// Wolverine handler for TenantDeletedEvent domain event.
/// Publishes the event to Kafka for other services to consume.
/// </summary>
public class TenantDeletedEventHandler
{
    private readonly ITenantEventPublisher _eventPublisher;
    private readonly ILogger<TenantDeletedEventHandler> _logger;

    public TenantDeletedEventHandler(
        ITenantEventPublisher eventPublisher,
        ILogger<TenantDeletedEventHandler> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    /// <summary>
    /// Handles the TenantDeletedEvent by publishing it to Kafka.
    /// This is called by Wolverine when the domain event is dispatched.
    /// </summary>
    public async Task Handle(TenantDeletedEvent @event, CancellationToken ct)
    {
        _logger.LogInformation(
            "Handling TenantDeletedEvent for tenant {TenantId}",
            @event.TenantId);

        // Publish to Kafka for other services to consume
        await _eventPublisher.PublishTenantDeletedAsync(@event, ct);

        _logger.LogInformation(
            "Successfully published TenantDeletedEvent to message broker for tenant {TenantId}",
            @event.TenantId);
    }
}
