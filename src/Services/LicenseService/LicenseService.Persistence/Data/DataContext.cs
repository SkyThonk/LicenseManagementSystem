using Common.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using LicenseService.Domain.Licenses;
using LicenseService.Domain.LicenseTypes;
using LicenseService.Domain.LicenseDocuments;
using LicenseService.Domain.LicenseStatusHistory;
using LicenseService.Domain.Renewals;

namespace LicenseService.Persistence.Data;

public class DataContext : DbContext
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public DataContext(DbContextOptions<DataContext> options, IDomainEventDispatcher domainEventDispatcher) : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    public DbSet<License> Licenses => Set<License>();
    public DbSet<LicenseType> LicenseTypes => Set<LicenseType>();
    public DbSet<LicenseDocument> LicenseDocuments => Set<LicenseDocument>();
    public DbSet<LicenseStatusHistoryEntry> LicenseStatusHistory => Set<LicenseStatusHistoryEntry>();
    public DbSet<Renewal> Renewals => Set<Renewal>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Apply AsNoTracking by default for all queries
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        // Make all the table to not generate uuid key on primary key by database.
        modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.IsPrimaryKey())
            .ToList()
            .ForEach(p => p.ValueGenerated = ValueGenerated.Never);

        // Add index for CreatedAt column in all entities implementing IEntity
        modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(IEntity).IsAssignableFrom(e.ClrType))
            .ToList()
            .ForEach(entityType =>
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasIndex("CreatedAt");
            });

        // Add global filter for IsDeleted
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(IEntity).IsAssignableFrom(e.ClrType)))
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
            var property = System.Linq.Expressions.Expression.Property(parameter, nameof(IEntity.IsDeleted));
            var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
            var lambda = System.Linq.Expressions.Expression.Lambda(condition, parameter);
            
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();

        // Collect and publish domain events within the same EF transaction (Wolverine outbox)
        var domainEntities = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IEntity entity && entity.DomainEvents.Count > 0)
            .ToList();

        if (domainEntities.Count > 0)
        {
            var events = domainEntities
                .SelectMany(e => ((IEntity)e.Entity).DomainEvents)
                .ToList();

            // Publish events; with Wolverine EF integration these will be enqueued in the outbox
            await _domainEventDispatcher.DispatchAsync(events, cancellationToken);

            // Clear to avoid duplicate dispatch on subsequent SaveChanges
            foreach (var entry in domainEntities)
            {
                ((IEntity)entry.Entity).ClearDomainEvent();
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entities = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Modified && e.Entity is IEntity);

        foreach (var entity in entities)
        {
            ((IEntity)entity.Entity).UpdatedAt = DateTime.UtcNow;
        }
    }
}
