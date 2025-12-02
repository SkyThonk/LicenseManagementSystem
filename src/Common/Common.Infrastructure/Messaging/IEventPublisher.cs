namespace Common.Infrastructure.Messaging;

/// <summary>
/// Interface for publishing integration events to message broker (Kafka).
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an integration event to the message broker.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event</typeparam>
    /// <param name="event">The event to publish</param>
    /// <param name="topic">Optional topic override, defaults to tenant-events</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync<TEvent>(TEvent @event, string? topic = null, CancellationToken cancellationToken = default)
        where TEvent : class;

    /// <summary>
    /// Publishes an integration event with a specific key for partitioning.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event</typeparam>
    /// <param name="event">The event to publish</param>
    /// <param name="key">The partition key (e.g., tenantId)</param>
    /// <param name="topic">Optional topic override</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync<TEvent>(TEvent @event, string key, string? topic = null, CancellationToken cancellationToken = default)
        where TEvent : class;
}
