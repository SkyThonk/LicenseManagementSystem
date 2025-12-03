using System.ComponentModel.DataAnnotations;

namespace NotificationService.Contracts.Notifications.GetNotification;

/// <summary>
/// Request to get notification details
/// </summary>
public record GetNotificationRequest(
    [Required(ErrorMessage = "Notification ID is required")]
    Guid NotificationId
);
