namespace NotificationService.Contracts.Notifications.GetNotification;

/// <summary>
/// Response with notification details
/// </summary>
public record GetNotificationResponse(
    Guid NotificationId,
    Guid TenantId,
    string Recipient,
    string? Subject,
    string Message,
    string NotificationType,
    string Status,
    DateTime CreatedAt,
    DateTime? SentAt,
    string? ErrorMessage
);
