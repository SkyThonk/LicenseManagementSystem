using Common.Domain.Abstractions;
using Common.Infrastructure.MultiTenancy;
using DocumentService.Application.Common.Interfaces;
using DocumentService.Application.Common.Interfaces.Repositories;
using DocumentService.Persistence.Data;
using DocumentService.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Wolverine;

namespace DocumentService.Persistence;

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
            options.ServicePrefix = "document";
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
                var applicationAssembly = Assembly.Load("DocumentService.Application");
                opts.Discovery.IncludeAssembly(applicationAssembly);
            }
            catch
            {
                // Assembly may not be loaded yet
            }
        });

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register repositories
        services.AddScoped<IDocumentRepository, DocumentRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Note: Database migrations for tenant databases are handled when tenants are created
        // via the TenantCreatedEvent handler, not at service startup

        return services;
    }
}
