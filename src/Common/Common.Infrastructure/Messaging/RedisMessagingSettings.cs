namespace Common.Infrastructure.Messaging;

/// <summary>
/// Settings for Redis Pub/Sub messaging.
/// </summary>
public class RedisMessagingSettings
{
    public const string SectionName = "Redis";
    
    /// <summary>
    /// The Redis connection string.
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";
    
    /// <summary>
    /// The channel name for tenant events.
    /// </summary>
    public string TenantEventsChannel { get; set; } = "tenant-events";
}
