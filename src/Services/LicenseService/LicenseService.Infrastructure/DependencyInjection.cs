using Common.Infrastructure.Messaging;
using LicenseService.Infrastructure.EventHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LicenseService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IHostApplicationBuilder builder
    )
    {
        // Configure Common infrastructure JwtSettings
        services.Configure<Common.Infrastructure.Authentication.JwtSettings>(
            builder.Configuration.GetSection(Common.Infrastructure.Authentication.JwtSettings.SectionName));

        // Register the Common.Application IJwtTokenGenerator implemented by Common.Infrastructure
        services.AddSingleton<Common.Application.Interfaces.Authentication.IJwtTokenGenerator, 
            Common.Infrastructure.Authentication.JwtTokenGenerator>();

        services.AddHttpContextAccessor();

        // Add Redis distributed cache using connection string
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("redis");
        });

        // Add Kafka event consumer for tenant events
        services.AddKafkaEventConsumer(builder.Configuration, "license-service");
        services.AddTenantEventHandler<LicenseTenantEventHandler>();

        return services;
    }
}
