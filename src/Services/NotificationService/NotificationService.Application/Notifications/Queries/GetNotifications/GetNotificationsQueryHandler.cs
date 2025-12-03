using Common.Application.Result;
using Common.Application.Interfaces;
using NotificationService.Application.Common.Interfaces.Repositories;
using NotificationService.Contracts.Notifications.GetNotifications;

namespace NotificationService.Application.Notifications.Queries.GetNotifications;

/// <summary>
/// Handler for getting notifications list.
/// Each tenant has their own isolated database, so no TenantId filtering is needed.
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
        var notifications = await _notificationRepo.GetAllAsync(ct);

        // Filter by status if specified
        if (!string.IsNullOrEmpty(query.Request.Status))
        {
            notifications = notifications
                .Where(n => n.Status.ToString().Equals(query.Request.Status, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var totalCount = notifications.Count;
        
        // Apply pagination
        var pagedNotifications = notifications
            .Skip((query.Request.Page - 1) * query.Request.PageSize)
            .Take(query.Request.PageSize)
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
            pagedNotifications,
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
