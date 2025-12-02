namespace Common.Domain.Abstractions;

public abstract class Entity<TEntityId> : IEntity
    where TEntityId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();
    
    protected Entity(TEntityId id, Guid? createdBy = null)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;

        IsDeleted = false;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
    }

    // For EF Core
    protected Entity() { }

    public TEntityId Id { get; init; } = default!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public Guid? CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }


    // Expose domain events in a safe way
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();


    // Method to add events from inside aggregates
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    protected void SetUpdatedBy(Guid? updatedBy)
    {
        UpdatedBy = updatedBy;
    }

    public void ClearDomainEvent()
    {
        _domainEvents.Clear();
    }
}