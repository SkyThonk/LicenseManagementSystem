using System.Text.Json;
using Common.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Common.Infrastructure.Messaging;

/// <summary>
/// Background service that consumes tenant events from Redis Pub/Sub and dispatches them to handlers.
/// Each service runs this consumer to react to tenant lifecycle events.
/// This consumer is designed to be resilient and will not crash the host if Redis is unavailable.
/// </summary>
public class RedisTenantEventConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RedisMessagingSettings _settings;
    private readonly ILogger<RedisTenantEventConsumer> _logger;
    private ConnectionMultiplexer? _redis;
    private ISubscriber? _subscriber;

    public RedisTenantEventConsumer(
        IServiceProvider serviceProvider,
        IOptions<RedisMessagingSettings> settings,
        ILogger<RedisTenantEventConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run the entire consumer logic in a try-catch to prevent crashes
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Try to connect to Redis
                if (!await TryConnectAsync(stoppingToken))
                {
                    _logger.LogWarning("Could not connect to Redis. Will retry in 10 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    continue;
                }

                _logger.LogInformation(
                    "Started consuming tenant events from Redis channel {Channel}",
                    _settings.TenantEventsChannel);

                // Subscribe to the tenant events channel
                await _subscriber!.SubscribeAsync(
                    RedisChannel.Literal(_settings.TenantEventsChannel),
                    async (channel, message) =>
                    {
                        if (message.HasValue)
                        {
                            await ProcessMessageAsync(message.ToString(), stoppingToken);
                        }
                    });

                // Keep running until cancelled or disconnected
                while (!stoppingToken.IsCancellationRequested && _redis?.IsConnected == true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }

                // If we get here, we lost connection - will retry
                if (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Lost connection to Redis. Reconnecting...");
                    CleanupConnection();
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Normal shutdown
                _logger.LogInformation("Redis tenant event consumer is shutting down");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Redis tenant event consumer. Will retry in 10 seconds...");
                CleanupConnection();
                
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private async Task<bool> TryConnectAsync(CancellationToken stoppingToken)
    {
        const int maxRetries = 60;
        
        for (int attempt = 1; attempt <= maxRetries && !stoppingToken.IsCancellationRequested; attempt++)
        {
            try
            {
                // Clean up any previous connection attempt
                CleanupConnection();
                
                var configOptions = BuildConfigurationOptions();
                
                _redis = await ConnectionMultiplexer.ConnectAsync(configOptions);
                _subscriber = _redis.GetSubscriber();
                
                // Test the connection by pinging
                var db = _redis.GetDatabase();
                await db.PingAsync();
                
                _logger.LogInformation("Successfully connected to Redis");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    "Failed to connect to Redis (attempt {Attempt}/{MaxRetries}): {Error}",
                    attempt,
                    maxRetries,
                    ex.Message);
                
                CleanupConnection();
            }

            if (attempt < maxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }

        return false;
    }

    private void CleanupConnection()
    {
        try
        {
            _subscriber = null;
            _redis?.Dispose();
            _redis = null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error cleaning up Redis connection");
        }
    }

    private ConfigurationOptions BuildConfigurationOptions()
    {
        // Parse the connection string to extract host, port, user, password
        var connectionString = _settings.ConnectionString;
        
        _logger.LogInformation("Building Redis configuration from connection string: {ConnectionStringLength} chars", 
            connectionString?.Length ?? 0);
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Redis connection string is not configured");
        }
        
        var configOptions = new ConfigurationOptions
        {
            AbortOnConnectFail = true,
            ConnectRetry = 1,
            ConnectTimeout = 5000,
            SyncTimeout = 5000,
            AsyncTimeout = 5000
        };

        // Parse connection string format: host:port,user=xxx,password=xxx
        var parts = connectionString.Split(',');
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;
            
            if (trimmed.Contains('='))
            {
                var keyValue = trimmed.Split('=', 2);
                var key = keyValue[0].Trim().ToLowerInvariant();
                var value = keyValue.Length > 1 ? keyValue[1].Trim() : "";
                
                switch (key)
                {
                    case "user":
                        configOptions.User = value;
                        break;
                    case "password":
                        configOptions.Password = value;
                        break;
                    case "abortconnect":
                        if (bool.TryParse(value, out var abort))
                            configOptions.AbortOnConnectFail = abort;
                        break;
                }
            }
            else if (trimmed.Contains(':'))
            {
                // This is host:port
                var hostPort = trimmed.Split(':');
                if (hostPort.Length == 2 && int.TryParse(hostPort[1], out var port))
                {
                    configOptions.EndPoints.Add(hostPort[0], port);
                    _logger.LogInformation("Added Redis endpoint: {Host}:{Port}", hostPort[0], port);
                }
            }
        }
        
        if (configOptions.EndPoints.Count == 0)
        {
            throw new InvalidOperationException($"No valid Redis endpoints found in connection string. String starts with: {connectionString.Substring(0, Math.Min(20, connectionString.Length))}...");
        }

        return configOptions;
    }

    private async Task ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        try
        {
            var envelope = JsonSerializer.Deserialize<IntegrationEventEnvelope>(message);
            if (envelope == null)
            {
                _logger.LogWarning("Received null envelope from Redis");
                return;
            }

            _logger.LogInformation(
                "Processing {EventType} from {SourceService}",
                envelope.EventType,
                envelope.SourceService);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetService<ITenantEventHandler>();

            if (handler == null)
            {
                _logger.LogWarning("No ITenantEventHandler registered, skipping event");
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

            _logger.LogInformation("Successfully processed {EventType}", envelope.EventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message from Redis: {Message}", message);
        }
    }

    public override void Dispose()
    {
        _redis?.Dispose();
        base.Dispose();
    }
}
