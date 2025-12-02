namespace NotificationService.Contracts.Notifications.GetNotification;

/// <summary>
/// Request to get notification details
/// </summary>
public record GetNotificationRequest(Guid NotificationId);
