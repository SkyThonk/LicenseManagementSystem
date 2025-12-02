namespace DocumentService.Domain.Documents;

/// <summary>
/// Strongly-typed ID for Document entity
/// </summary>
public readonly record struct DocumentId(Guid Value)
{
    public static DocumentId New() => new(Guid.NewGuid());
    public static DocumentId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}
