using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Common.Infrastructure.Migration;

/// <summary>
/// Service to create tenant-specific databases dynamically.
/// Creates a new database for each tenant using the naming pattern: {service}_{tenantId}
/// </summary>
public interface ITenantDatabaseCreator
{
    /// <summary>
    /// Creates a new database for the tenant and applies migrations
    /// </summary>
    Task<string> CreateTenantDatabaseAsync<TContext>(
        Guid tenantId, 
        string servicePrefix,
        CancellationToken cancellationToken = default) where TContext : DbContext;
}

public class TenantDatabaseCreator : ITenantDatabaseCreator
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenantDatabaseCreator> _logger;

    public TenantDatabaseCreator(
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        ILogger<TenantDatabaseCreator> logger)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<string> CreateTenantDatabaseAsync<TContext>(
        Guid tenantId,
        string servicePrefix,
        CancellationToken cancellationToken = default) where TContext : DbContext
    {
        // Get the base connection string
        var baseConnectionString = _configuration.GetConnectionString("SQL") 
            ?? throw new InvalidOperationException("SQL connection string not configured");

        // Parse the connection string to extract components
        var builder = new NpgsqlConnectionStringBuilder(baseConnectionString);
        
        // Create the tenant-specific database name
        var tenantDbName = $"{servicePrefix}_{tenantId:N}".ToLowerInvariant();
        
        _logger.LogInformation(
            "Creating tenant database {DatabaseName} for tenant {TenantId}",
            tenantDbName,
            tenantId);

        // First, connect to the default database to create the new database
        var adminConnectionString = baseConnectionString;
        
        try
        {
            await using var adminConnection = new NpgsqlConnection(adminConnectionString);
            await adminConnection.OpenAsync(cancellationToken);

            // Check if database already exists
            var checkDbCommand = adminConnection.CreateCommand();
            checkDbCommand.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{tenantDbName}'";
            var exists = await checkDbCommand.ExecuteScalarAsync(cancellationToken);

            if (exists == null)
            {
                // Create the database
                var createDbCommand = adminConnection.CreateCommand();
                createDbCommand.CommandText = $"CREATE DATABASE \"{tenantDbName}\"";
                await createDbCommand.ExecuteNonQueryAsync(cancellationToken);
                
                _logger.LogInformation("Created database {DatabaseName}", tenantDbName);
            }
            else
            {
                _logger.LogInformation("Database {DatabaseName} already exists", tenantDbName);
            }
        }
        catch (PostgresException ex) when (ex.SqlState == "42P04") // duplicate_database
        {
            _logger.LogInformation("Database {DatabaseName} already exists", tenantDbName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create database {DatabaseName}", tenantDbName);
            throw;
        }

        // Build the tenant-specific connection string
        builder.Database = tenantDbName;
        var tenantConnectionString = builder.ConnectionString;

        // Now apply migrations to the new database
        try
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseNpgsql(tenantConnectionString);

            // Get domain event dispatcher if available
            var dispatcher = _serviceProvider.GetService(typeof(Common.Domain.Abstractions.IDomainEventDispatcher)) 
                as Common.Domain.Abstractions.IDomainEventDispatcher;

            // Create context with the tenant connection
            TContext? context = null;
            
            // Try to create with dispatcher first
            var constructors = typeof(TContext).GetConstructors();
            foreach (var ctor in constructors)
            {
                var parameters = ctor.GetParameters();
                if (parameters.Length == 2 && 
                    parameters[0].ParameterType.IsAssignableFrom(typeof(DbContextOptions<TContext>)) &&
                    parameters[1].ParameterType == typeof(Common.Domain.Abstractions.IDomainEventDispatcher))
                {
                    if (dispatcher != null)
                    {
                        context = (TContext)ctor.Invoke(new object[] { optionsBuilder.Options, dispatcher });
                        break;
                    }
                }
                else if (parameters.Length == 1 && 
                         parameters[0].ParameterType.IsAssignableFrom(typeof(DbContextOptions<TContext>)))
                {
                    context = (TContext)ctor.Invoke(new object[] { optionsBuilder.Options });
                    break;
                }
            }

            if (context == null)
            {
                throw new InvalidOperationException($"Could not create DbContext of type {typeof(TContext).Name}");
            }

            using (context)
            {
                _logger.LogInformation("Applying migrations to database {DatabaseName}", tenantDbName);
                await context.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("Migrations applied successfully to {DatabaseName}", tenantDbName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply migrations to database {DatabaseName}", tenantDbName);
            throw;
        }

        return tenantConnectionString;
    }
}
