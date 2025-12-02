namespace NotificationService.Contracts.Notifications.SendNotification;

/// <summary>
/// Response after queuing a notification
/// </summary>
public record SendNotificationResponse(
    Guid NotificationId,
    string Status,
    DateTime CreatedAt
);
