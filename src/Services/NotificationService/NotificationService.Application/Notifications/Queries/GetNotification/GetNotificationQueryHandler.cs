using Common.Application.Result;
using Common.Application.Interfaces;
using NotificationService.Application.Common.Interfaces.Repositories;
using NotificationService.Contracts.Notifications.GetNotification;
using NotificationService.Domain.Notifications;

namespace NotificationService.Application.Notifications.Queries.GetNotification;

/// <summary>
/// Handler for getting notification details
/// </summary>
public class GetNotificationQueryHandler : IQueryHandler<GetNotificationQuery, GetNotificationResponse>
{
    private readonly INotificationRepository _notificationRepo;

    public GetNotificationQueryHandler(INotificationRepository notificationRepo)
    {
        _notificationRepo = notificationRepo;
    }

    public async Task<Result<GetNotificationResponse>> Handle(GetNotificationQuery query, CancellationToken ct)
    {
        var notification = await _notificationRepo.GetByIdAsync(
            new NotificationId(query.Request.NotificationId), 
            query.TenantId, 
            ct);

        if (notification == null)
        {
            return Result<GetNotificationResponse>.Failure(
                new NotFoundError("Notification not found"));
        }

        return Result<GetNotificationResponse>.Success(new GetNotificationResponse(
            notification.Id.Value,
            notification.TenantId,
            notification.Recipient,
            notification.Subject,
            notification.Message,
            notification.NotificationType.ToString(),
            notification.Status.ToString(),
            notification.CreatedAt,
            notification.SentAt,
            notification.ErrorMessage
        ));
    }
}

/// <summary>
/// Query for getting notification details
/// </summary>
public record GetNotificationQuery(
    GetNotificationRequest Request,
    Guid TenantId
);
