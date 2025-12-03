using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LicenseService.Persistence.Data;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using System.Reflection;
using LicenseService.Persistence.Common;
using LicenseService.Application.Common.Interfaces;
using LicenseService.Application.Common.Interfaces.Repositories;
using LicenseService.Persistence.Repositories;
using Common.Domain.Abstractions;
using Common.Infrastructure.Migration;

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

        // Add Wolverine to the host and configure its EF Core integration
        builder.UseWolverine(opts =>
        {
            // Configure Wolverine durable message persistence in PostgreSQL
            opts.PersistMessagesWithPostgresql(connectionString);

            opts.Policies.UseDurableLocalQueues();

            // You can also add more specific policies for resilience
            opts.Policies.UseDurableOutboxOnAllSendingEndpoints();

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

        // Add EF Core with PostgreSQL
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(connectionString));

        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Repositories
        services.AddScoped<ILicenseRepository, LicenseRepository>();
        services.AddScoped<ILicenseTypeRepository, LicenseTypeRepository>();
        services.AddScoped<ILicenseDocumentRepository, LicenseDocumentRepository>();
        services.AddScoped<ILicenseStatusHistoryRepository, LicenseStatusHistoryRepository>();
        services.AddScoped<IRenewalRepository, RenewalRepository>();

        // Add database migration service to run at startup
        services.AddSimpleDatabaseMigration<DataContext>();

        return services;
    }
}
