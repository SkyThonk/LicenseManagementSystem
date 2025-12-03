using LicenseManagement.Web.Constants;
using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.ViewModels.Notifications;
using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationApiService _apiService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationApiService apiService, ILogger<NotificationService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<NotificationListViewModel> GetNotificationsAsync(int page = 1, int pageSize = 10, bool? unreadOnly = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = $"?page={page}&pageSize={pageSize}";
            if (unreadOnly == true)
            {
                queryParams += "&unreadOnly=true";
            }

            var response = await _apiService.GetAsync<GetNotificationsApiResponse>(
                $"{ApiEndpoints.Notification.GetAll}{queryParams}",
                cancellationToken);

            if (response != null)
            {
                var items = response.Notifications.Select(MapToListItemViewModel).ToList();
                return new NotificationListViewModel
                {
                    Notifications = new PaginatedList<NotificationItemViewModel>(
                        items,
                        response.TotalCount,
                        response.Page,
                        response.PageSize),
                    StatusFilter = unreadOnly == true ? "unread" : null
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching notifications list");
        }

        return new NotificationListViewModel
        {
            Notifications = new PaginatedList<NotificationItemViewModel>([], 0, page, pageSize),
            StatusFilter = unreadOnly == true ? "unread" : null
        };
    }

    public async Task<int> GetUnreadCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _apiService.GetAsync<GetUnreadCountApiResponse>(
                ApiEndpoints.Notification.UnreadCount,
                cancellationToken);

            return response?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching unread notification count");
            return 0;
        }
    }

    public async Task<bool> MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = string.Format(ApiEndpoints.Notification.MarkAsRead, id);
            var response = await _apiService.PatchAsync<object, MarkAsReadApiResponse>(
                endpoint, new { }, cancellationToken);
            return response?.Success ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read {NotificationId}", id);
            return false;
        }
    }

    public async Task<bool> MarkAllAsReadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _apiService.PatchAsync<object, MarkAsReadApiResponse>(
                ApiEndpoints.Notification.MarkAllAsRead, new { }, cancellationToken);
            return response?.Success ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            return false;
        }
    }

    public async Task<bool> DeleteNotificationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = string.Format(ApiEndpoints.Notification.Delete, id);
            var response = await _apiService.DeleteAsync<DeleteNotificationApiResponse>(endpoint, cancellationToken);
            return response?.Success ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
            return false;
        }
    }

    private static NotificationItemViewModel MapToListItemViewModel(NotificationApiDto dto)
    {
        return new NotificationItemViewModel
        {
            Id = dto.NotificationId,
            Recipient = dto.Recipient,
            Subject = dto.Subject ?? "No Subject",
            NotificationType = dto.NotificationType,
            Status = dto.Status,
            CreatedAt = dto.CreatedAt,
            SentAt = dto.SentAt,
            ErrorMessage = null
        };
    }
}

#region API DTOs

internal class GetNotificationsApiResponse
{
    public List<NotificationApiDto> Notifications { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

internal class NotificationApiDto
{
    public Guid NotificationId { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
}

internal class GetUnreadCountApiResponse
{
    public int Count { get; set; }
}

internal class MarkAsReadApiResponse
{
    public bool Success { get; set; }
}

internal class DeleteNotificationApiResponse
{
    public bool Success { get; set; }
}

#endregion
