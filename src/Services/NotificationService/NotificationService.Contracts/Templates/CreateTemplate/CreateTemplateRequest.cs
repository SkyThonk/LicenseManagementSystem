using System.ComponentModel.DataAnnotations;

namespace NotificationService.Contracts.Templates.CreateTemplate;

/// <summary>
/// Request to create a notification template
/// </summary>
public record CreateTemplateRequest(
    [Required(ErrorMessage = "Template name is required")]
    [MaxLength(200, ErrorMessage = "Template name cannot exceed 200 characters")]
    string TemplateName,

    [Required(ErrorMessage = "Subject is required")]
    [MaxLength(500, ErrorMessage = "Subject cannot exceed 500 characters")]
    string Subject,

    [Required(ErrorMessage = "Body is required")]
    string Body,

    [Required(ErrorMessage = "Notification type is required")]
    [RegularExpression("^(Email|Sms|Push)$", ErrorMessage = "Invalid notification type")]
    string NotificationType
);
