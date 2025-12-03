using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationService.Contracts.Templates.UpdateTemplate;

/// <summary>
/// Request to update a notification template
/// </summary>
public record UpdateTemplateRequest(
    [property: JsonIgnore]
    Guid TemplateId,

    [MaxLength(200, ErrorMessage = "Template name cannot exceed 200 characters")]
    string? TemplateName = null,

    [MaxLength(500, ErrorMessage = "Subject cannot exceed 500 characters")]
    string? Subject = null,

    string? Body = null,

    [RegularExpression("^(Email|Sms|Push)$", ErrorMessage = "Invalid notification type")]
    string? NotificationType = null
);
