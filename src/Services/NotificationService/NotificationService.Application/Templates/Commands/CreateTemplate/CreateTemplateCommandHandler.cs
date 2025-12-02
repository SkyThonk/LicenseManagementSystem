using Common.Application.Result;
using Common.Application.Interfaces;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Interfaces.Repositories;
using NotificationService.Contracts.Templates.CreateTemplate;
using NotificationService.Domain.Common.Enums;
using NotificationService.Domain.Templates;

namespace NotificationService.Application.Templates.Commands.CreateTemplate;

/// <summary>
/// Handler for creating a notification template
/// </summary>
public class CreateTemplateCommandHandler : ICommandHandler<CreateTemplateCommand, CreateTemplateResponse>
{
    private readonly INotificationTemplateRepository _templateRepo;
    private readonly IUnitOfWork _uow;

    public CreateTemplateCommandHandler(
        INotificationTemplateRepository templateRepo,
        IUnitOfWork uow)
    {
        _templateRepo = templateRepo;
        _uow = uow;
    }

    public async Task<Result<CreateTemplateResponse>> Handle(CreateTemplateCommand command, CancellationToken ct)
    {
        var request = command.Request;
        
        // Parse notification type
        if (!Enum.TryParse<NotificationType>(request.NotificationType, true, out var notificationType))
        {
            return Result<CreateTemplateResponse>.Failure(
                new ValidationError($"Invalid notification type: {request.NotificationType}. Valid values are: Email, SMS, Push"));
        }

        // Check if template name already exists for this tenant
        if (await _templateRepo.ExistsByNameAsync(request.TemplateName, command.TenantId, ct))
        {
            return Result<CreateTemplateResponse>.Failure(
                new ValidationError($"Template with name '{request.TemplateName}' already exists"));
        }

        // Create the template
        var template = NotificationTemplate.Create(
            tenantId: command.TenantId,
            templateName: request.TemplateName,
            subject: request.Subject,
            body: request.Body,
            notificationType: notificationType,
            createdBy: command.UserId
        );

        _templateRepo.Add(template);
        await _uow.SaveChangesAsync(ct);

        return Result<CreateTemplateResponse>.Success(new CreateTemplateResponse(
            template.Id.Value,
            template.TemplateName,
            template.CreatedAt
        ));
    }
}

/// <summary>
/// Command for creating a template
/// </summary>
public record CreateTemplateCommand(
    CreateTemplateRequest Request,
    Guid TenantId,
    Guid? UserId
);
