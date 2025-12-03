using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.MultiTenancy;

/// <summary>
/// Extension methods for registering multi-tenancy services.
/// </summary>
public static class MultiTenancyExtensions
{
    /// <summary>
    /// Adds multi-tenancy support with separate databases per tenant.
    /// The tenant is resolved from JWT claims in each HTTP request.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type to use for tenant databases.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration containing TenantDatabase settings.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMultiTenancy<TContext>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TContext : DbContext
    {
        // Register settings
        services.Configure<TenantDatabaseSettings>(
            configuration.GetSection(TenantDatabaseSettings.SectionName));

        // Register HTTP context accessor for JWT claim access
        services.AddHttpContextAccessor();

        // Register tenant context accessor (scoped per request)
        services.AddScoped<ITenantContext, TenantContextAccessor>();

        // Register tenant-aware DbContext factory
        services.AddScoped<IDbContextFactory<TContext>, TenantDbContextFactory<TContext>>();

        // Register the DbContext as scoped, resolved from the factory
        services.AddScoped<TContext>(sp =>
        {
            var factory = sp.GetRequiredService<IDbContextFactory<TContext>>();
            return factory.CreateDbContext();
        });

        return services;
    }

    /// <summary>
    /// Adds multi-tenancy support with a central database for Wolverine outbox.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The central database connection string.</returns>
    public static string GetCentralConnectionString(IConfiguration configuration)
    {
        var settings = configuration.GetSection(TenantDatabaseSettings.SectionName)
            .Get<TenantDatabaseSettings>();

        return settings?.CentralConnectionString
            ?? configuration.GetConnectionString("SQL")
            ?? throw new InvalidOperationException("Central connection string not configured.");
    }
}
