namespace NotificationService.Contracts.Templates.GetTemplates;

/// <summary>
/// Response with list of templates
/// </summary>
public record GetTemplatesResponse(IReadOnlyList<TemplateDto> Templates);

public record TemplateDto(
    Guid TemplateId,
    string TemplateName,
    string Subject,
    string NotificationType,
    bool IsActive,
    DateTime CreatedAt
);
