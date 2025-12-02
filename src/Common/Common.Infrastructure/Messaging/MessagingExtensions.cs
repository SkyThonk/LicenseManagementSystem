using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Messaging;

/// <summary>
/// Extension methods for registering messaging services.
/// </summary>
public static class MessagingExtensions
{
    /// <summary>
    /// Adds Kafka event publishing capability to the service.
    /// Call this in services that need to publish events (e.g., TenantService).
    /// </summary>
    public static IServiceCollection AddKafkaEventPublisher(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        services.Configure<KafkaSettings>(configuration.GetSection(KafkaSettings.SectionName));
        
        services.AddSingleton<IEventPublisher>(sp =>
        {
            var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<KafkaSettings>>();
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<KafkaEventPublisher>>();
            return new KafkaEventPublisher(settings, logger, serviceName);
        });

        return services;
    }

    /// <summary>
    /// Adds Kafka event consumer capability to the service.
    /// Call this in services that need to consume tenant events.
    /// The service must also register an ITenantEventHandler implementation.
    /// </summary>
    public static IServiceCollection AddKafkaEventConsumer(
        this IServiceCollection services,
        IConfiguration configuration,
        string consumerGroupId)
    {
        services.Configure<KafkaSettings>(options =>
        {
            configuration.GetSection(KafkaSettings.SectionName).Bind(options);
            options.ConsumerGroupId = consumerGroupId;
        });

        services.AddHostedService<KafkaTenantEventConsumer>();

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
