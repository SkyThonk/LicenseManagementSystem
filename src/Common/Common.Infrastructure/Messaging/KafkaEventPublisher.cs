using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Infrastructure.Messaging;

/// <summary>
/// Kafka-based event publisher for integration events.
/// </summary>
public class KafkaEventPublisher : IEventPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaEventPublisher> _logger;
    private readonly string _serviceName;
    private bool _disposed;

    public KafkaEventPublisher(
        IOptions<KafkaSettings> settings,
        ILogger<KafkaEventPublisher> logger,
        string serviceName = "Unknown")
    {
        _settings = settings.Value;
        _logger = logger;
        _serviceName = serviceName;

        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            Acks = Acks.All, // Ensure durability
            EnableIdempotence = true, // Exactly-once semantics
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000
        };

        // Configure security if specified
        if (!string.IsNullOrEmpty(_settings.SecurityProtocol) && _settings.SecurityProtocol != "PLAINTEXT")
        {
            config.SecurityProtocol = Enum.Parse<SecurityProtocol>(_settings.SecurityProtocol.Replace("_", ""));
            
            if (!string.IsNullOrEmpty(_settings.SaslMechanism))
            {
                config.SaslMechanism = Enum.Parse<SaslMechanism>(_settings.SaslMechanism.Replace("-", "").Replace("_", ""));
                config.SaslUsername = _settings.SaslUsername;
                config.SaslPassword = _settings.SaslPassword;
            }
        }

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string? topic = null, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        var key = Guid.NewGuid().ToString();
        await PublishAsync(@event, key, topic, cancellationToken);
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string key, string? topic = null, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        var targetTopic = topic ?? _settings.TenantEventsTopic;
        var eventType = typeof(TEvent).Name;

        var envelope = new IntegrationEventEnvelope
        {
            EventType = eventType,
            Payload = JsonSerializer.Serialize(@event),
            Timestamp = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid().ToString(),
            SourceService = _serviceName
        };

        var message = new Message<string, string>
        {
            Key = key,
            Value = JsonSerializer.Serialize(envelope)
        };

        try
        {
            var result = await _producer.ProduceAsync(targetTopic, message, cancellationToken);
            
            _logger.LogInformation(
                "Published {EventType} to topic {Topic}, partition {Partition}, offset {Offset}",
                eventType,
                result.Topic,
                result.Partition.Value,
                result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to publish {EventType} to topic {Topic}", eventType, targetTopic);
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
        _disposed = true;
    }
}
