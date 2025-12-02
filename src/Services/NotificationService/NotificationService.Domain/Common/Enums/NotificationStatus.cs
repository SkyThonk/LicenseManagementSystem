namespace NotificationService.Domain.Common.Enums;

/// <summary>
/// Status of the notification
/// </summary>
public enum NotificationStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2,
    Cancelled = 3
}
