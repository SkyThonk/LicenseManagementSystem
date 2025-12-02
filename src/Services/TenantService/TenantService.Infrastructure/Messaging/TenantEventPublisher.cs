using System.Text.Json;
using Common.Domain.Events;
using TenantService.Application.Common.Interfaces.Messaging;
using Microsoft.Extensions.Logging;

namespace TenantService.Infrastructure.Messaging;

/// <summary>
/// Kafka-based implementation of tenant event publisher.
/// Publishes tenant lifecycle events to the message broker.
/// </summary>
public class TenantEventPublisher : ITenantEventPublisher
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<TenantEventPublisher> _logger;
    private const string TenantEventsTopic = "tenant-events";

    public TenantEventPublisher(
        IKafkaProducer kafkaProducer,
        ILogger<TenantEventPublisher> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    public async Task PublishTenantCreatedAsync(TenantCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        var envelope = CreateEnvelope(@event);
        var key = @event.TenantId.ToString();
        
        await _kafkaProducer.ProduceAsync(TenantEventsTopic, key, JsonSerializer.Serialize(envelope));
        
        _logger.LogInformation(
            "Published TenantCreatedEvent for tenant {TenantId} ({AgencyCode})",
            @event.TenantId,
            @event.AgencyCode);
    }

    public async Task PublishTenantUpdatedAsync(TenantUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        var envelope = CreateEnvelope(@event);
        var key = @event.TenantId.ToString();
        
        await _kafkaProducer.ProduceAsync(TenantEventsTopic, key, JsonSerializer.Serialize(envelope));
        
        _logger.LogInformation(
            "Published TenantUpdatedEvent for tenant {TenantId}",
            @event.TenantId);
    }

    public async Task PublishTenantDeletedAsync(TenantDeletedEvent @event, CancellationToken cancellationToken = default)
    {
        var envelope = CreateEnvelope(@event);
        var key = @event.TenantId.ToString();
        
        await _kafkaProducer.ProduceAsync(TenantEventsTopic, key, JsonSerializer.Serialize(envelope));
        
        _logger.LogInformation(
            "Published TenantDeletedEvent for tenant {TenantId}",
            @event.TenantId);
    }

    private static IntegrationEventEnvelope CreateEnvelope<TEvent>(TEvent @event) where TEvent : class
    {
        return new IntegrationEventEnvelope
        {
            EventType = typeof(TEvent).Name,
            Payload = JsonSerializer.Serialize(@event),
            Timestamp = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid().ToString(),
            SourceService = "TenantService"
        };
    }
}

/// <summary>
/// Wrapper for integration events to include metadata for message routing.
/// </summary>
public class IntegrationEventEnvelope
{
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? CorrelationId { get; set; }
    public string? SourceService { get; set; }
}
