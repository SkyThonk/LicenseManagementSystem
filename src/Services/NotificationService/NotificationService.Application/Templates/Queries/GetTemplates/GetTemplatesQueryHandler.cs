using Common.Application.Result;
using Common.Application.Interfaces;
using NotificationService.Application.Common.Interfaces.Repositories;
using NotificationService.Contracts.Templates.GetTemplates;

namespace NotificationService.Application.Templates.Queries.GetTemplates;

/// <summary>
/// Handler for getting templates list
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
        var templates = query.Request.ActiveOnly
            ? await _templateRepo.GetActiveTemplatesAsync(query.TenantId, ct)
            : await _templateRepo.GetByTenantIdAsync(query.TenantId, ct);

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

        return Result<GetTemplatesResponse>.Success(new GetTemplatesResponse(templateDtos));
    }
}

/// <summary>
/// Query for getting templates list
/// </summary>
public record GetTemplatesQuery(
    GetTemplatesRequest Request,
    Guid TenantId
);
