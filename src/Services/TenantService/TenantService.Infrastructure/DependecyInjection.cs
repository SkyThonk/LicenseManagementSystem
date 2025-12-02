using TenantService.Application.Common.Interfaces.Authentication;
using TenantService.Infrastructure.Messaging;
using TenantService.Infrastructure.Authentication;
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
        services.Configure<KafkaSettings>(builder.Configuration.GetSection(KafkaSettings.SectionName));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        // Add Redis distributed cache using connection string
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("redis");
        });

        // Add Kafka producer
        services.AddSingleton<IKafkaProducer, KafkaProducer>();
        services.AddSingleton<IKafkaAdmin, KafkaAdmin>();

        return services;
    }
}

