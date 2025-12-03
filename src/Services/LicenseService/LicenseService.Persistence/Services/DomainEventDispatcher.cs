using Common.Domain.Abstractions;

namespace LicenseService.Persistence.Services;

internal sealed class DomainEventDispatcher(Wolverine.IMessageBus messageBus) : IDomainEventDispatcher
{
    private readonly Wolverine.IMessageBus _messageBus = messageBus;

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var @event in domainEvents)
        {
            // Publish as in-process events; Wolverine will outbox them within the EF transaction
            await _messageBus.PublishAsync(@event);
        }
    }
}
