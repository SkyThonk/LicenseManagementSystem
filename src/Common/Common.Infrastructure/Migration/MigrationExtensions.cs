using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Infrastructure.Migration;

/// <summary>
/// Extension methods for registering database migration services
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Add database migration service that runs at application startup
    /// </summary>
    /// <typeparam name="TContext">The DbContext type to migrate</typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="useTenantIsolation">Whether to use tenant-based database isolation</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDatabaseMigration<TContext>(
        this IServiceCollection services,
        bool useTenantIsolation = false) where TContext : DbContext
    {
        if (useTenantIsolation)
        {
            // Register tenant provider for fetching tenant connection strings
            services.AddHttpClient<ITenantProvider, TenantServiceProvider>();
        }

        // Register the migration service as a hosted service
        services.AddHostedService(sp =>
        {
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<DatabaseMigrationService<TContext>>>();
            return new DatabaseMigrationService<TContext>(sp, logger, useTenantIsolation);
        });

        return services;
    }

    /// <summary>
    /// Add simple database migration that ensures database exists and applies pending migrations
    /// </summary>
    /// <typeparam name="TContext">The DbContext type to migrate</typeparam>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddSimpleDatabaseMigration<TContext>(
        this IServiceCollection services) where TContext : DbContext
    {
        return services.AddDatabaseMigration<TContext>(useTenantIsolation: false);
    }
}
