namespace Common.Infrastructure.Messaging;

/// <summary>
/// Wrapper for integration events to include metadata for message routing.
/// </summary>
public class IntegrationEventEnvelope
{
    /// <summary>
    /// The type name of the event for deserialization
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// The JSON serialized event payload
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// When the event was created
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Correlation ID for tracing
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Source service that published the event
    /// </summary>
    public string? SourceService { get; set; }
}
