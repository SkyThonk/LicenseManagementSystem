using Common.Application.Result;
using Common.Application.Interfaces;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Interfaces.Repositories;
using NotificationService.Contracts.Notifications.SendNotification;
using NotificationService.Domain.Common.Enums;
using NotificationService.Domain.Notifications;

namespace NotificationService.Application.Notifications.Commands.SendNotification;

/// <summary>
/// Handler for sending a notification
/// </summary>
public class SendNotificationCommandHandler : ICommandHandler<SendNotificationCommand, SendNotificationResponse>
{
    private readonly INotificationRepository _notificationRepo;
    private readonly INotificationTemplateRepository _templateRepo;
    private readonly IUnitOfWork _uow;

    public SendNotificationCommandHandler(
        INotificationRepository notificationRepo,
        INotificationTemplateRepository templateRepo,
        IUnitOfWork uow)
    {
        _notificationRepo = notificationRepo;
        _templateRepo = templateRepo;
        _uow = uow;
    }

    public async Task<Result<SendNotificationResponse>> Handle(SendNotificationCommand command, CancellationToken ct)
    {
        var request = command.Request;
        
        // Parse notification type
        if (!Enum.TryParse<NotificationType>(request.NotificationType, true, out var notificationType))
        {
            return Result<SendNotificationResponse>.Failure(
                new ValidationError($"Invalid notification type: {request.NotificationType}. Valid values are: Email, SMS, Push"));
        }

        string subject = request.Subject ?? string.Empty;
        string message = request.Message;

        // If template is specified, render it
        if (request.TemplateId.HasValue)
        {
            var template = await _templateRepo.GetByIdAsync(
                new Domain.Templates.NotificationTemplateId(request.TemplateId.Value), 
                command.TenantId, 
                ct);

            if (template == null)
            {
                return Result<SendNotificationResponse>.Failure(
                    new NotFoundError("Template not found"));
            }

            if (!template.IsActive)
            {
                return Result<SendNotificationResponse>.Failure(
                    new ValidationError("Template is not active"));
            }

            var placeholders = request.Placeholders ?? new Dictionary<string, string>();
            var (renderedSubject, renderedBody) = template.Render(placeholders);
            subject = renderedSubject;
            message = renderedBody;
        }

        // Create the notification
        var notification = Notification.Create(
            tenantId: command.TenantId,
            recipient: request.Recipient,
            message: message,
            notificationType: notificationType,
            subject: subject,
            templateId: request.TemplateId,
            createdBy: command.UserId
        );

        _notificationRepo.Add(notification);
        await _uow.SaveChangesAsync(ct);

        return Result<SendNotificationResponse>.Success(new SendNotificationResponse(
            notification.Id.Value,
            notification.Status.ToString(),
            notification.CreatedAt
        ));
    }
}

/// <summary>
/// Command for sending a notification
/// </summary>
public record SendNotificationCommand(
    SendNotificationRequest Request,
    Guid TenantId,
    Guid? UserId
);
