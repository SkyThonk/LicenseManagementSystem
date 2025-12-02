namespace NotificationService.Contracts.Templates.CreateTemplate;

/// <summary>
/// Request to create a notification template
/// </summary>
public record CreateTemplateRequest(
    string TemplateName,
    string Subject,
    string Body,
    string NotificationType
);
