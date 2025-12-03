using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Common.Infrastructure.Messaging;

/// <summary>
/// Redis-based event publisher for tenant events.
/// Uses Redis Pub/Sub for real-time event distribution.
/// </summary>
public class RedisEventPublisher : IEventPublisher, IDisposable
{
    private readonly ConnectionMultiplexer _redis;
    private readonly ISubscriber _subscriber;
    private readonly RedisMessagingSettings _settings;
    private readonly ILogger<RedisEventPublisher> _logger;
    private readonly string _serviceName;
    private bool _disposed;

    public RedisEventPublisher(
        IOptions<RedisMessagingSettings> settings,
        ILogger<RedisEventPublisher> logger,
        string serviceName = "Unknown")
    {
        _settings = settings.Value;
        _logger = logger;
        _serviceName = serviceName;

        var configOptions = BuildConfigurationOptions();
        
        _redis = ConnectionMultiplexer.Connect(configOptions);
        _subscriber = _redis.GetSubscriber();
        
        _logger.LogInformation("Redis event publisher initialized for service {ServiceName}", _serviceName);
    }

    private ConfigurationOptions BuildConfigurationOptions()
    {
        var connectionString = _settings.ConnectionString;
        
        var configOptions = new ConfigurationOptions
        {
            AbortOnConnectFail = false,
            ConnectRetry = 3,
            ConnectTimeout = 5000
        };

        // Parse connection string format: host:port,user=xxx,password=xxx
        var parts = connectionString.Split(',');
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (trimmed.Contains('='))
            {
                var keyValue = trimmed.Split('=', 2);
                var key = keyValue[0].Trim().ToLowerInvariant();
                var value = keyValue[1].Trim();
                
                switch (key)
                {
                    case "user":
                        configOptions.User = value;
                        break;
                    case "password":
                        configOptions.Password = value;
                        break;
                    case "abortconnect":
                        configOptions.AbortOnConnectFail = bool.Parse(value);
                        break;
                }
            }
            else if (trimmed.Contains(':'))
            {
                // This is host:port
                var hostPort = trimmed.Split(':');
                configOptions.EndPoints.Add(hostPort[0], int.Parse(hostPort[1]));
            }
        }

        return configOptions;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string? channel = null, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        var key = Guid.NewGuid().ToString();
        await PublishAsync(@event, key, channel, cancellationToken);
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string key, string? channel = null, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        var targetChannel = channel ?? _settings.TenantEventsChannel;
        var eventType = typeof(TEvent).Name;

        var envelope = new IntegrationEventEnvelope
        {
            EventType = eventType,
            Payload = JsonSerializer.Serialize(@event),
            Timestamp = DateTime.UtcNow,
            CorrelationId = key,
            SourceService = _serviceName
        };

        var message = JsonSerializer.Serialize(envelope);

        try
        {
            var subscriberCount = await _subscriber.PublishAsync(
                RedisChannel.Literal(targetChannel), 
                message);

            _logger.LogInformation(
                "Published {EventType} to channel {Channel} (received by {SubscriberCount} subscribers)",
                eventType,
                targetChannel,
                subscriberCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish {EventType} to channel {Channel}", eventType, targetChannel);
            throw;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _redis?.Dispose();
            _disposed = true;
        }
    }
}
