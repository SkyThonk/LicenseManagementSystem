namespace NotificationService.Domain.Templates;

/// <summary>
/// Strongly-typed ID for NotificationTemplate entity
/// </summary>
public readonly record struct NotificationTemplateId(Guid Value)
{
    public static NotificationTemplateId New() => new(Guid.NewGuid());
    public static NotificationTemplateId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}
