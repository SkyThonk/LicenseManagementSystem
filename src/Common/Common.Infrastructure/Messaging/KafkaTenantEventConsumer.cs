using System.Text.Json;
using Common.Domain.Events;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Infrastructure.Messaging;

/// <summary>
/// Background service that consumes tenant events from Kafka and dispatches them to handlers.
/// Each service runs this consumer to react to tenant lifecycle events.
/// </summary>
public class KafkaTenantEventConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaTenantEventConsumer> _logger;
    private readonly IConsumer<string, string> _consumer;

    public KafkaTenantEventConsumer(
        IServiceProvider serviceProvider,
        IOptions<KafkaSettings> settings,
        ILogger<KafkaTenantEventConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _settings = settings.Value;
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.ConsumerGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false, // Manual commit for reliability
            EnablePartitionEof = true
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

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_settings.TenantEventsTopic);
        
        _logger.LogInformation(
            "Started consuming tenant events from topic {Topic} with consumer group {GroupId}",
            _settings.TenantEventsTopic,
            _settings.ConsumerGroupId);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    
                    if (consumeResult.IsPartitionEOF)
                    {
                        continue;
                    }

                    await ProcessMessageAsync(consumeResult.Message.Value, stoppingToken);
                    
                    _consumer.Commit(consumeResult);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer is shutting down");
        }
        finally
        {
            _consumer.Close();
        }
    }

    private async Task ProcessMessageAsync(string messageValue, CancellationToken cancellationToken)
    {
        try
        {
            var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope>(messageValue);
            if (envelope == null)
            {
                _logger.LogWarning("Received null envelope from Kafka");
                return;
            }

            _logger.LogInformation(
                "Processing {EventType} from {SourceService} at {Timestamp}",
                envelope.EventType,
                envelope.SourceService,
                envelope.Timestamp);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetService<ITenantEventHandler>();
            
            if (handler == null)
            {
                _logger.LogWarning("No ITenantEventHandler registered, skipping event processing");
                return;
            }

            switch (envelope.EventType)
            {
                case nameof(TenantCreatedEvent):
                    var createdEvent = JsonSerializer.Deserialize<TenantCreatedEvent>(envelope.Payload);
                    if (createdEvent != null)
                    {
                        await handler.HandleTenantCreatedAsync(createdEvent, cancellationToken);
                    }
                    break;

                case nameof(TenantUpdatedEvent):
                    var updatedEvent = JsonSerializer.Deserialize<TenantUpdatedEvent>(envelope.Payload);
                    if (updatedEvent != null)
                    {
                        await handler.HandleTenantUpdatedAsync(updatedEvent, cancellationToken);
                    }
                    break;

                case nameof(TenantDeletedEvent):
                    var deletedEvent = JsonSerializer.Deserialize<TenantDeletedEvent>(envelope.Payload);
                    if (deletedEvent != null)
                    {
                        await handler.HandleTenantDeletedAsync(deletedEvent, cancellationToken);
                    }
                    break;

                default:
                    _logger.LogWarning("Unknown event type: {EventType}", envelope.EventType);
                    break;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize message from Kafka");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message from Kafka");
            throw; // Re-throw to prevent commit and allow retry
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
