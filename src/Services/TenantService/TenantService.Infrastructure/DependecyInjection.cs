using TenantService.Application.Common.Interfaces.Authentication;
using TenantService.Application.Common.Interfaces.Messaging;
using TenantService.Infrastructure.Messaging;
using TenantService.Infrastructure.Authentication;
// No Common.* using directives here to avoid naming conflicts; use fully-qualified types instead
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
        services.Configure<KafkaSettings>(builder.Configuration.GetSection(KafkaSettings.SectionName));

        // TenantService-specific token generator registration
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        // Also register the Common.Application IJwtTokenGenerator implemented by Common.Infrastructure
        services.AddSingleton<Common.Application.Interfaces.Authentication.IJwtTokenGenerator, Common.Infrastructure.Authentication.JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        
        // Register email service
        services.AddScoped<TenantService.Application.Common.Interfaces.Services.IEmailService, Services.EmailService>();

        // Add Redis distributed cache using connection string
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("redis");
        });

        // Add Kafka producer and event publisher
        services.AddSingleton<IKafkaProducer, KafkaProducer>();
        services.AddSingleton<IKafkaAdmin, KafkaAdmin>();
        services.AddScoped<ITenantEventPublisher, TenantEventPublisher>();

        return services;
    }
}

