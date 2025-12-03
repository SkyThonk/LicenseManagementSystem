namespace NotificationService.Contracts.Notifications.GetNotification;

/// <summary>
/// Response with notification details.
/// TenantId is not included as each tenant has their own isolated database.
/// </summary>
public record GetNotificationResponse(
    Guid NotificationId,
    string Recipient,
    string? Subject,
    string Message,
    string NotificationType,
    string Status,
    DateTime CreatedAt,
    DateTime? SentAt,
    string? ErrorMessage
);
