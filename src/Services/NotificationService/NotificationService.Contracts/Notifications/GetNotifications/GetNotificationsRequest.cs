namespace NotificationService.Contracts.Notifications.GetNotifications;

/// <summary>
/// Request to get notifications for a tenant
/// </summary>
public record GetNotificationsRequest(
    int Page = 1,
    int PageSize = 20,
    string? Status = null
);
