using TenantService.Application.Common.Interfaces.Messaging;
using TenantService.Infrastructure.Messaging;
using TenantService.Infrastructure.Authentication;
using Common.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace TenantService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IHostApplicationBuilder builder
        )
    {
        services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
        // Also configure Common infrastructure JwtSettings so the Common JwtTokenGenerator
        // can receive IOptions<JwtSettings> when injected.
        services.Configure<Common.Infrastructure.Authentication.JwtSettings>(builder.Configuration.GetSection(Common.Infrastructure.Authentication.JwtSettings.SectionName));
        
        // Configure Redis messaging settings
        services.Configure<RedisMessagingSettings>(options =>
        {
            var redisConnectionString = builder.Configuration.GetConnectionString("redis");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                options.ConnectionString = redisConnectionString;
            }
        });

        // TenantService-specific token generator registration
        services.AddSingleton<TenantService.Application.Common.Interfaces.Authentication.IJwtTokenGenerator, JwtTokenGenerator>();
        // Also register the Common.Application IJwtTokenGenerator implemented by Common.Infrastructure
        services.AddSingleton<Common.Application.Interfaces.Authentication.IJwtTokenGenerator, Common.Infrastructure.Authentication.JwtTokenGenerator>();
        services.AddSingleton<TenantService.Application.Common.Interfaces.Authentication.IPasswordHasher, PasswordHasher>();
        services.AddHttpContextAccessor();
        services.AddScoped<Common.Application.Interfaces.Authentication.IUserContext, Common.Infrastructure.Authentication.UserContext>();
        
        // Register email service
        services.AddScoped<TenantService.Application.Common.Interfaces.Services.IEmailService, Services.EmailService>();

        // Add Redis distributed cache using connection string
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("redis");
        });

        // Add Redis producer and event publisher
        services.AddSingleton<IRedisProducer, RedisProducer>();
        services.AddScoped<ITenantEventPublisher, TenantEventPublisher>();

        return services;
    }
}

