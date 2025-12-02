using Common.Domain.Abstractions;
using DocumentService.Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DocumentService.Persistence.Data;

public class DataContext : DbContext
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public DataContext(DbContextOptions<DataContext> options, IDomainEventDispatcher domainEventDispatcher) : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    public DbSet<Document> Documents => Set<Document>();

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

        foreach (var entityEntry in entities)
        {
            var entity = (IEntity)entityEntry.Entity;
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }

    public static bool IsNotDeleted(object entity)
    {
        return EF.Property<bool>(entity, "IsDeleted") == false;
    }
}
