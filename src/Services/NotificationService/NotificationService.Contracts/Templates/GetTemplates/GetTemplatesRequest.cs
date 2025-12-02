namespace NotificationService.Contracts.Templates.GetTemplates;

/// <summary>
/// Request to get templates for a tenant
/// </summary>
public record GetTemplatesRequest(bool ActiveOnly = false);
