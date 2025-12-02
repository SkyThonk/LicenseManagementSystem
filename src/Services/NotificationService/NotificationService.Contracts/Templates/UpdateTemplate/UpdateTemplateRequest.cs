namespace NotificationService.Contracts.Templates.UpdateTemplate;

/// <summary>
/// Request to update a notification template
/// </summary>
public record UpdateTemplateRequest(
    Guid TemplateId,
    string? TemplateName = null,
    string? Subject = null,
    string? Body = null,
    string? NotificationType = null
);
