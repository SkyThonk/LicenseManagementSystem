using Common.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
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
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;

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
                var applicationAssembly = Assembly.Load("NotificationService.Application");
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

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
