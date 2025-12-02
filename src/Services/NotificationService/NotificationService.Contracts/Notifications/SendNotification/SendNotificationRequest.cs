namespace NotificationService.Contracts.Notifications.SendNotification;

/// <summary>
/// Request to send a notification
/// </summary>
public record SendNotificationRequest(
    string Recipient,
    string Message,
    string NotificationType,
    string? Subject = null,
    Guid? TemplateId = null,
    Dictionary<string, string>? Placeholders = null
);
