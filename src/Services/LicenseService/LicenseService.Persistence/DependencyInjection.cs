using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LicenseService.Persistence.Data;
using LicenseService.Persistence.Common;
using LicenseService.Application.Common.Interfaces;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Persistence.Repositories;
using LicenseService.Persistence.Services;
using Common.Domain.Abstractions;
using Common.Infrastructure.MultiTenancy;
using System.Reflection;
using Wolverine;

namespace LicenseService.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IHostApplicationBuilder builder
    )
    {
        var connectionString = builder.Configuration.GetConnectionString("SQL")
               ?? throw new InvalidOperationException("Connection string 'SQL' not found.");

        // Add Wolverine for in-memory message bus (no outbox persistence for tenant-aware service)
        builder.UseWolverine(opts =>
        {
            // Use in-memory queues only - no PostgreSQL persistence
            opts.Policies.UseDurableLocalQueues();

            // Ensure Wolverine discovers handlers in the Application assembly
            try
            {
                var applicationAssembly = Assembly.Load("LicenseService.Application");
                opts.Discovery.IncludeAssembly(applicationAssembly);
            }
            catch
            {
                // If the assembly can't be loaded by name, Wolverine will still scan the entry assembly.
            }
        });

        // Configure tenant database settings with service prefix
        services.Configure<TenantDatabaseSettings>(options =>
        {
            options.ServicePrefix = "license";
            options.CentralConnectionString = connectionString;
            options.UseSeparateDatabases = true;
        });

        // Add multi-tenancy support - DbContext is resolved per-request based on JWT tenant claim
        services.AddMultiTenancy<DataContext>(builder.Configuration);

        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register DomainEventDispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register Repositories
        services.AddScoped<ILicenseRepository, LicenseRepository>();
        services.AddScoped<ILicenseTypeRepository, LicenseTypeRepository>();
        services.AddScoped<ILicenseDocumentRepository, LicenseDocumentRepository>();
        services.AddScoped<ILicenseStatusHistoryRepository, LicenseStatusHistoryRepository>();
        services.AddScoped<IRenewalRepository, RenewalRepository>();

        // Note: Database migrations for tenant databases are handled when tenants are created
        // via the TenantCreatedEvent handler, not at service startup

        return services;
    }
}
