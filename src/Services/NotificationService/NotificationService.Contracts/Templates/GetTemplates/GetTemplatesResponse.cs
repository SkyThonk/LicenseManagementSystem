namespace NotificationService.Contracts.Templates.GetTemplates;

/// <summary>
/// Response with paginated list of templates
/// </summary>
public record GetTemplatesResponse(
    IReadOnlyList<TemplateDto> Templates,
    int TotalCount,
    int Page,
    int PageSize
);

public record TemplateDto(
    Guid TemplateId,
    string TemplateName,
    string Subject,
    string NotificationType,
    bool IsActive,
    DateTime CreatedAt
);
