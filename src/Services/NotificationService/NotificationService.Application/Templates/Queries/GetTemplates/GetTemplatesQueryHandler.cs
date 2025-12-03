using Common.Application.Result;
using Common.Application.Interfaces;
using NotificationService.Application.Common.Interfaces.Repositories;
using NotificationService.Contracts.Templates.GetTemplates;

namespace NotificationService.Application.Templates.Queries.GetTemplates;

/// <summary>
/// Handler for getting paginated templates list.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// Pagination happens at the SQL level for efficiency.
/// </summary>
public class GetTemplatesQueryHandler : IQueryHandler<GetTemplatesQuery, GetTemplatesResponse>
{
    private readonly INotificationTemplateRepository _templateRepo;

    public GetTemplatesQueryHandler(INotificationTemplateRepository templateRepo)
    {
        _templateRepo = templateRepo;
    }

    public async Task<Result<GetTemplatesResponse>> Handle(GetTemplatesQuery query, CancellationToken ct)
    {
        // Pagination and filtering happens at SQL level in repository
        var (templates, totalCount) = await _templateRepo.GetPaginatedAsync(
            query.Request.Page,
            query.Request.PageSize,
            query.Request.ActiveOnly,
            ct);

        var templateDtos = templates
            .Select(t => new TemplateDto(
                t.Id.Value,
                t.TemplateName,
                t.Subject,
                t.NotificationType.ToString(),
                t.IsActive,
                t.CreatedAt
            ))
            .ToList();

        return Result<GetTemplatesResponse>.Success(new GetTemplatesResponse(
            templateDtos,
            totalCount,
            query.Request.Page,
            query.Request.PageSize
        ));
    }
}

/// <summary>
/// Query for getting templates list
/// </summary>
public record GetTemplatesQuery(
    GetTemplatesRequest Request
);
