namespace NotificationService.Contracts.Templates.CreateTemplate;

/// <summary>
/// Response after creating a template
/// </summary>
public record CreateTemplateResponse(
    Guid TemplateId,
    string TemplateName,
    DateTime CreatedAt
);
