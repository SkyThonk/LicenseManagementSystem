using System.ComponentModel.DataAnnotations;

namespace NotificationService.Contracts.Templates.GetTemplates;

/// <summary>
/// Request to get paginated templates for a tenant
/// </summary>
public record GetTemplatesRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    int Page = 1,

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    int PageSize = 10,

    bool ActiveOnly = false
);
