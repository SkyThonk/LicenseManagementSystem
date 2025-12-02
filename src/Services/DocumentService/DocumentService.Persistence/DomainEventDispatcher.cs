using Common.Domain.Abstractions;
using Wolverine;

namespace DocumentService.Persistence;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMessageBus _messageBus;

    public DomainEventDispatcher(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _messageBus.PublishAsync(domainEvent);
        }
    }
}
