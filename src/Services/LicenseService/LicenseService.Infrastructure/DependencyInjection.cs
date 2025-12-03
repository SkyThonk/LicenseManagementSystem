using Common.Infrastructure.Messaging;
using Common.Infrastructure.Migration;
using LicenseService.Application.Common.Interfaces;
using LicenseService.Infrastructure.BackgroundJobs;
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

        // Register tenant database creator for dynamic database provisioning
        services.AddScoped<ITenantDatabaseCreator, TenantDatabaseCreator>();

        // Add Redis event consumer for tenant events
        services.AddRedisEventConsumer(builder.Configuration);
        services.AddTenantEventHandler<LicenseTenantEventHandler>();

        // Configure and register license renewal background job
        services.Configure<LicenseRenewalJobSettings>(
            builder.Configuration.GetSection(LicenseRenewalJobSettings.SectionName));
        services.AddScoped<ILicenseRenewalProcessor, LicenseRenewalProcessor>();
        services.AddHostedService<LicenseRenewalBackgroundJob>();

        return services;
    }
}
