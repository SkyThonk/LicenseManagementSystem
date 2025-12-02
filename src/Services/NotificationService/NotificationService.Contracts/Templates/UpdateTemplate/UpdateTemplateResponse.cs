namespace NotificationService.Contracts.Templates.UpdateTemplate;

/// <summary>
/// Response after updating a template
/// </summary>
public record UpdateTemplateResponse(
    Guid TemplateId,
    string TemplateName,
    DateTime UpdatedAt
);
