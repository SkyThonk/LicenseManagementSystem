using Common.Application.Interfaces.Authentication;
using Common.Infrastructure.Authentication;
using Common.Infrastructure.Messaging;
using Common.Infrastructure.Migration;
using DocumentService.Application.Common.Interfaces.Services;
using DocumentService.Infrastructure.EventHandlers;
using DocumentService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DocumentService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IHostApplicationBuilder builder
    )
    {
        // Configure Common infrastructure JwtSettings for authentication
        services.Configure<Common.Infrastructure.Authentication.JwtSettings>(
            builder.Configuration.GetSection(Common.Infrastructure.Authentication.JwtSettings.SectionName));

        // Register Common JWT token generator
        services.AddSingleton<Common.Application.Interfaces.Authentication.IJwtTokenGenerator,
            Common.Infrastructure.Authentication.JwtTokenGenerator>();

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        // Register file storage service
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // Add Redis distributed cache using connection string
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("redis");
        });

        // Register tenant database creator for dynamic database provisioning
        services.AddScoped<ITenantDatabaseCreator, TenantDatabaseCreator>();

        // Register Redis tenant event consumer
        services.AddRedisEventConsumer(builder.Configuration);
        services.AddTenantEventHandler<DocumentTenantEventHandler>();

        return services;
    }
}
