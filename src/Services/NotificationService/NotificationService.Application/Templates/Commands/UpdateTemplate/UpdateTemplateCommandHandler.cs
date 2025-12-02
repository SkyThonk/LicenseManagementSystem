using Common.Application.Result;
using Common.Application.Interfaces;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Interfaces.Repositories;
using NotificationService.Contracts.Templates.UpdateTemplate;
using NotificationService.Domain.Common.Enums;
using NotificationService.Domain.Templates;

namespace NotificationService.Application.Templates.Commands.UpdateTemplate;

/// <summary>
/// Handler for updating a notification template
/// </summary>
public class UpdateTemplateCommandHandler : ICommandHandler<UpdateTemplateCommand, UpdateTemplateResponse>
{
    private readonly INotificationTemplateRepository _templateRepo;
    private readonly IUnitOfWork _uow;

    public UpdateTemplateCommandHandler(
        INotificationTemplateRepository templateRepo,
        IUnitOfWork uow)
    {
        _templateRepo = templateRepo;
        _uow = uow;
    }

    public async Task<Result<UpdateTemplateResponse>> Handle(UpdateTemplateCommand command, CancellationToken ct)
    {
        var request = command.Request;
        
        var template = await _templateRepo.GetByIdAsync(
            new NotificationTemplateId(request.TemplateId), 
            command.TenantId, 
            ct);

        if (template == null)
        {
            return Result<UpdateTemplateResponse>.Failure(
                new NotFoundError("Template not found"));
        }

        // Parse notification type if provided
        NotificationType? notificationType = null;
        if (!string.IsNullOrEmpty(request.NotificationType))
        {
            if (!Enum.TryParse<NotificationType>(request.NotificationType, true, out var parsed))
            {
                return Result<UpdateTemplateResponse>.Failure(
                    new ValidationError($"Invalid notification type: {request.NotificationType}. Valid values are: Email, SMS, Push"));
            }
            notificationType = parsed;
        }

        // Check if new template name conflicts with existing
        if (!string.IsNullOrEmpty(request.TemplateName) && 
            request.TemplateName != template.TemplateName &&
            await _templateRepo.ExistsByNameAsync(request.TemplateName, command.TenantId, ct))
        {
            return Result<UpdateTemplateResponse>.Failure(
                new ValidationError($"Template with name '{request.TemplateName}' already exists"));
        }

        // Update the template
        template.Update(
            templateName: request.TemplateName,
            subject: request.Subject,
            body: request.Body,
            notificationType: notificationType,
            updatedBy: command.UserId
        );

        _templateRepo.Update(template);
        await _uow.SaveChangesAsync(ct);

        return Result<UpdateTemplateResponse>.Success(new UpdateTemplateResponse(
            template.Id.Value,
            template.TemplateName,
            template.UpdatedAt
        ));
    }
}

/// <summary>
/// Command for updating a template
/// </summary>
public record UpdateTemplateCommand(
    UpdateTemplateRequest Request,
    Guid TenantId,
    Guid? UserId
);
