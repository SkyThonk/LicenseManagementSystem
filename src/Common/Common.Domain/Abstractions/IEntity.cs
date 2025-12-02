namespace Common.Domain.Abstractions;

public interface IEntity
{
    DateTime UpdatedAt { get; set; }

    IReadOnlyList<IDomainEvent> DomainEvents { get; }
 
    void ClearDomainEvent();

    bool IsDeleted { get; set; }
}
