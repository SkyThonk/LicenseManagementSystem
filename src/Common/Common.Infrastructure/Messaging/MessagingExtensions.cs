using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Messaging;

/// <summary>
/// Extension methods for registering messaging services.
/// </summary>
public static class MessagingExtensions
{
    /// <summary>
    /// Adds Redis event publishing capability to the service.
    /// Call this in services that need to publish events (e.g., TenantService).
    /// </summary>
    public static IServiceCollection AddRedisEventPublisher(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        services.Configure<RedisMessagingSettings>(options =>
        {
            var redisConnectionString = configuration.GetConnectionString("redis");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                options.ConnectionString = redisConnectionString;
            }
        });
        
        services.AddSingleton<IEventPublisher>(sp =>
        {
            var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RedisMessagingSettings>>();
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<RedisEventPublisher>>();
            return new RedisEventPublisher(settings, logger, serviceName);
        });

        return services;
    }

    /// <summary>
    /// Adds Redis event consumer capability to the service.
    /// Call this in services that need to consume tenant events.
    /// The service must also register an ITenantEventHandler implementation.
    /// </summary>
    public static IServiceCollection AddRedisEventConsumer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RedisMessagingSettings>(options =>
        {
            var redisConnectionString = configuration.GetConnectionString("redis");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                options.ConnectionString = redisConnectionString;
            }
        });

        services.AddHostedService<RedisTenantEventConsumer>();

        return services;
    }

    /// <summary>
    /// Registers the tenant event handler implementation.
    /// </summary>
    public static IServiceCollection AddTenantEventHandler<THandler>(this IServiceCollection services)
        where THandler : class, ITenantEventHandler
    {
        services.AddScoped<ITenantEventHandler, THandler>();
        return services;
    }
}
