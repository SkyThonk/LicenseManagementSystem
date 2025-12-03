using System.Text.Json;
using Common.Domain.Events;
using Common.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace TenantService.Infrastructure.Messaging;

/// <summary>
/// Redis-based producer for tenant events.
/// </summary>
public interface IRedisProducer
{
    Task PublishAsync(string channel, string message);
}

public class RedisProducer : IRedisProducer, IDisposable
{
    private ConnectionMultiplexer? _redis;
    private ISubscriber? _subscriber;
    private readonly RedisMessagingSettings _settings;
    private readonly ILogger<RedisProducer> _logger;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public RedisProducer(IOptions<RedisMessagingSettings> settings, ILogger<RedisProducer> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _logger.LogInformation("Redis producer initialized (lazy connection) for: {ConnectionString}", 
            _settings.ConnectionString);
    }

    private async Task EnsureConnectedAsync()
    {
        if (_redis?.IsConnected == true && _subscriber != null)
            return;

        await _connectionLock.WaitAsync();
        try
        {
            if (_redis?.IsConnected == true && _subscriber != null)
                return;
                
            _redis?.Dispose();
            
            var configOptions = BuildConfigurationOptions();
            
            _redis = await ConnectionMultiplexer.ConnectAsync(configOptions);
            _subscriber = _redis.GetSubscriber();
            
            _logger.LogInformation("Redis producer connected");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect Redis producer");
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private ConfigurationOptions BuildConfigurationOptions()
    {
        var connectionString = _settings.ConnectionString;
        
        var configOptions = new ConfigurationOptions
        {
            AbortOnConnectFail = true,
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

    public async Task PublishAsync(string channel, string message)
    {
        try
        {
            await EnsureConnectedAsync();
            
            var subscriberCount = await _subscriber!.PublishAsync(
                RedisChannel.Literal(channel), 
                message);

            _logger.LogInformation(
                "Message published to channel {Channel} (received by {SubscriberCount} subscribers)", 
                channel, 
                subscriberCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to channel {Channel}", channel);
            throw;
        }
    }

    public void Dispose()
    {
        _redis?.Dispose();
    }
}
