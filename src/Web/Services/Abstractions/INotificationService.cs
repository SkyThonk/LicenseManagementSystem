using LicenseManagement.Web.ViewModels.Notifications;

namespace LicenseManagement.Web.Services.Abstractions;

public interface INotificationService
{
    Task<NotificationListViewModel> GetNotificationsAsync(int page = 1, int pageSize = 10, bool? unreadOnly = null, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> MarkAllAsReadAsync(CancellationToken cancellationToken = default);
    Task<bool> DeleteNotificationAsync(Guid id, CancellationToken cancellationToken = default);
}
