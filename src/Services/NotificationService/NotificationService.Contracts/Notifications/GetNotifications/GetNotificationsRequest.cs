using System.ComponentModel.DataAnnotations;

namespace NotificationService.Contracts.Notifications.GetNotifications;

/// <summary>
/// Request to get notifications for a tenant
/// </summary>
public record GetNotificationsRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    int Page = 1,

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    int PageSize = 20,

    [RegularExpression("^(Pending|Sent|Failed)?$", ErrorMessage = "Invalid status")]
    string? Status = null
);
