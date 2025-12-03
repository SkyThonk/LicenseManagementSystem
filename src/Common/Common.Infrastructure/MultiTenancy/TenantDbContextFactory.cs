using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.MultiTenancy;

/// <summary>
/// Factory for creating tenant-aware DbContext instances.
/// Uses the tenant context to determine the correct database connection.
/// </summary>
public class TenantDbContextFactory<TContext> : IDbContextFactory<TContext>
    where TContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITenantContext _tenantContext;

    public TenantDbContextFactory(
        IServiceProvider serviceProvider,
        ITenantContext tenantContext)
    {
        _serviceProvider = serviceProvider;
        _tenantContext = tenantContext;
    }

    public TContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        optionsBuilder.UseNpgsql(_tenantContext.ConnectionString);

        // Get the domain event dispatcher if available
        var domainEventDispatcher = _serviceProvider.GetService(typeof(Common.Domain.Abstractions.IDomainEventDispatcher));

        // Try to create the context with the appropriate constructor
        if (domainEventDispatcher != null)
        {
            return (TContext)Activator.CreateInstance(
                typeof(TContext),
                optionsBuilder.Options,
                domainEventDispatcher)!;
        }

        return (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options)!;
    }
}
