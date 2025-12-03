using System.ComponentModel.DataAnnotations;

namespace NotificationService.Contracts.Notifications.SendNotification;

/// <summary>
/// Request to send a notification
/// </summary>
public record SendNotificationRequest(
    [Required(ErrorMessage = "Recipient is required")]
    [MaxLength(255, ErrorMessage = "Recipient cannot exceed 255 characters")]
    string Recipient,

    [Required(ErrorMessage = "Message is required")]
    string Message,

    [Required(ErrorMessage = "Notification type is required")]
    [RegularExpression("^(Email|Sms|Push)$", ErrorMessage = "Invalid notification type")]
    string NotificationType,

    [MaxLength(500, ErrorMessage = "Subject cannot exceed 500 characters")]
    string? Subject = null,

    Guid? TemplateId = null,

    Dictionary<string, string>? Placeholders = null
);
