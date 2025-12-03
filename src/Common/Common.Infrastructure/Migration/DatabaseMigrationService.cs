using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.Migration;

/// <summary>
/// Background service that runs database migrations for all tenants at startup
/// </summary>
public class DatabaseMigrationService<TContext> : BackgroundService where TContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigrationService<TContext>> _logger;
    private readonly bool _useTenantIsolation;

    public DatabaseMigrationService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseMigrationService<TContext>> logger,
        bool useTenantIsolation = false)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _useTenantIsolation = useTenantIsolation;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting database migration for {Context}...", typeof(TContext).Name);

        try
        {
            if (_useTenantIsolation)
            {
                await MigrateAllTenantsAsync(stoppingToken);
            }
            else
            {
                await MigrateSingleDatabaseAsync(stoppingToken);
            }

            _logger.LogInformation("Database migration completed successfully for {Context}", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database migration for {Context}", typeof(TContext).Name);
            throw;
        }
    }

    private async Task MigrateSingleDatabaseAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        _logger.LogInformation("Ensuring database exists and applying migrations...");
        
        // Create database if not exists and apply migrations
        await context.Database.MigrateAsync(ct);
        
        _logger.LogInformation("Database migration completed for default database");
    }

    private async Task MigrateAllTenantsAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var tenantProvider = scope.ServiceProvider.GetService<ITenantProvider>();

        if (tenantProvider == null)
        {
            _logger.LogWarning("No ITenantProvider registered. Running single database migration.");
            await MigrateSingleDatabaseAsync(ct);
            return;
        }

        var tenants = await tenantProvider.GetAllTenantsAsync(ct);
        var tenantList = tenants.ToList();

        _logger.LogInformation("Found {TenantCount} tenants to migrate", tenantList.Count);

        foreach (var tenant in tenantList)
        {
            try
            {
                _logger.LogInformation("Migrating database for tenant: {TenantName} ({TenantId})", 
                    tenant.Name, tenant.Id);

                await MigrateTenantDatabaseAsync(tenant, ct);

                _logger.LogInformation("Successfully migrated database for tenant: {TenantName}", tenant.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to migrate database for tenant: {TenantName} ({TenantId})", 
                    tenant.Name, tenant.Id);
                // Continue with other tenants
            }
        }
    }

    private async Task MigrateTenantDatabaseAsync(TenantInfo tenant, CancellationToken ct)
    {
        // Create a new DbContext with the tenant's connection string
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        optionsBuilder.UseNpgsql(tenant.ConnectionString);

        // Get the domain event dispatcher (may be null)
        var domainEventDispatcher = _serviceProvider.GetService(typeof(Common.Domain.Abstractions.IDomainEventDispatcher));

        // Try to create context with tenant-specific connection
        // First try (options, dispatcher) constructor, then (options) only
        TContext? context = null;
        try
        {
            // Try constructor with domain event dispatcher
            var constructorWithDispatcher = typeof(TContext).GetConstructor(new[] 
            { 
                optionsBuilder.Options.GetType().BaseType!,  // DbContextOptions<TContext>
                typeof(Common.Domain.Abstractions.IDomainEventDispatcher) 
            });

            if (constructorWithDispatcher != null && domainEventDispatcher != null)
            {
                context = (TContext)constructorWithDispatcher.Invoke(new object[] 
                { 
                    optionsBuilder.Options, 
                    domainEventDispatcher 
                });
            }
            else
            {
                // Try constructor with just options
                context = (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options)!;
            }
        }
        catch
        {
            // Fallback: use DbContextFactory if available
            var scope = _serviceProvider.CreateScope();
            var factory = scope.ServiceProvider.GetService<IDbContextFactory<TContext>>();
            if (factory != null)
            {
                context = await factory.CreateDbContextAsync(ct);
            }
        }

        if (context == null)
        {
            throw new InvalidOperationException($"Could not create DbContext of type {typeof(TContext).Name}");
        }

        using (context)
        {
            await context.Database.MigrateAsync(ct);
        }
    }
}
