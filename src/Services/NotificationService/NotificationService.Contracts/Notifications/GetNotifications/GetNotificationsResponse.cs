namespace NotificationService.Contracts.Notifications.GetNotifications;

/// <summary>
/// Response with list of notifications
/// </summary>
public record GetNotificationsResponse(
    IReadOnlyList<NotificationDto> Notifications,
    int TotalCount,
    int Page,
    int PageSize
);

public record NotificationDto(
    Guid NotificationId,
    string Recipient,
    string? Subject,
    string NotificationType,
    string Status,
    DateTime CreatedAt,
    DateTime? SentAt
);
