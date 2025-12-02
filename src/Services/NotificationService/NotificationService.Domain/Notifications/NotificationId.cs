namespace NotificationService.Domain.Notifications;

/// <summary>
/// Strongly-typed ID for Notification entity
/// </summary>
public readonly record struct NotificationId(Guid Value)
{
    public static NotificationId New() => new(Guid.NewGuid());
    public static NotificationId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}
