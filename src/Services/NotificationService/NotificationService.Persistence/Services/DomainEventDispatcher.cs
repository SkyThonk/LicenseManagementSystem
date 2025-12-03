using Common.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace NotificationService.Persistence.Services;

/// <summary>
/// Domain event dispatcher using Wolverine IMessageBus.
/// Events are dispatched in-memory without outbox persistence.
/// </summary>
internal sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IMessageBus messageBus, ILogger<DomainEventDispatcher> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var @event in domainEvents)
        {
            _logger.LogInformation("Dispatching domain event: {EventType}", @event.GetType().Name);
            await _messageBus.PublishAsync(@event);
        }
    }
}
