using Common.Domain.Abstractions;
using Common.Infrastructure.MultiTenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Interfaces.Repositories;
using NotificationService.Persistence.Common;
using NotificationService.Persistence.Data;
using NotificationService.Persistence.Repositories;
using NotificationService.Persistence.Services;
using System.Reflection;
using Wolverine;

namespace NotificationService.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IHostApplicationBuilder builder
    )
    {
        var connectionString = builder.Configuration.GetConnectionString("SQL")
               ?? throw new InvalidOperationException("Connection string 'SQL' not found.");

        // Configure tenant database settings with service prefix
        services.Configure<TenantDatabaseSettings>(options =>
        {
            options.ServicePrefix = "notification";
            options.CentralConnectionString = connectionString;
            options.UseSeparateDatabases = true;
        });

        // Add multi-tenancy support - DbContext is resolved per-request based on JWT tenant claim
        services.AddMultiTenancy<DataContext>(builder.Configuration);

        // Add Wolverine for messaging (without PostgreSQL outbox - using in-memory messaging only)
        builder.UseWolverine(opts =>
        {
            opts.Policies.UseDurableLocalQueues();
            
            try
            {
                var applicationAssembly = Assembly.Load("NotificationService.Application");
                opts.Discovery.IncludeAssembly(applicationAssembly);
            }
            catch
            {
                // Assembly may not be loaded yet
            }
        });

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Note: Database migrations for tenant databases are handled when tenants are created
        // via the TenantCreatedEvent handler, not at service startup

        return services;
    }
}
