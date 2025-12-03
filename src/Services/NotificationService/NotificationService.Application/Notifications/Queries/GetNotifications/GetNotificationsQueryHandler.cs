using Common.Application.Result;
using Common.Application.Interfaces;
using NotificationService.Application.Common.Interfaces.Repositories;
using NotificationService.Contracts.Notifications.GetNotifications;

namespace NotificationService.Application.Notifications.Queries.GetNotifications;

/// <summary>
/// Handler for getting notifications list.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
/// Pagination is performed at the SQL level for efficiency.
/// </summary>
public class GetNotificationsQueryHandler : IQueryHandler<GetNotificationsQuery, GetNotificationsResponse>
{
    private readonly INotificationRepository _notificationRepo;

    public GetNotificationsQueryHandler(INotificationRepository notificationRepo)
    {
        _notificationRepo = notificationRepo;
    }

    public async Task<Result<GetNotificationsResponse>> Handle(GetNotificationsQuery query, CancellationToken ct)
    {
        // Pagination and filtering happens at SQL level in repository
        var (notifications, totalCount) = await _notificationRepo.GetPaginatedAsync(
            query.Request.Page,
            query.Request.PageSize,
            query.Request.Status,
            ct);

        var notificationDtos = notifications
            .Select(n => new NotificationDto(
                n.Id.Value,
                n.Recipient,
                n.Subject,
                n.NotificationType.ToString(),
                n.Status.ToString(),
                n.CreatedAt,
                n.SentAt
            ))
            .ToList();

        return Result<GetNotificationsResponse>.Success(new GetNotificationsResponse(
            notificationDtos,
            totalCount,
            query.Request.Page,
            query.Request.PageSize
        ));
    }
}

/// <summary>
/// Query for getting notifications list
/// </summary>
public record GetNotificationsQuery(
    GetNotificationsRequest Request
);
