using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TenantService.Persistence.Data;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using System.Reflection;
using TenantService.Persistence.Common;
using TenantService.Application.Common.Interfaces;
using TenantService.Application.Common.Interfaces.Repositories;
using TenantService.Persistence.Repositories;
using TenantService.Persistence.Services;
using Common.Domain.Abstractions;

namespace TenantService.Persistence;

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
                var applicationAssembly = Assembly.Load("TenantService.Application");
                opts.Discovery.IncludeAssembly(applicationAssembly);
            }
            catch
            {
                // If the assembly can't be loaded by name, Wolverine will still scan the entry assembly.
            }
        });

        // Register DbContext with Wolverine integration
        builder.Services.AddDbContextWithWolverineIntegration<DataContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register repositories - only Tenant repository for this microservice
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

