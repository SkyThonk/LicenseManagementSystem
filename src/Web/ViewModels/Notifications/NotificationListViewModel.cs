using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.ViewModels.Notifications;

/// <summary>
/// Notification list page view model
/// </summary>
public class NotificationListViewModel : BaseViewModel
{
    public PaginatedList<NotificationItemViewModel> Notifications { get; set; } = new();
    public string? SearchQuery { get; set; }
    public string? StatusFilter { get; set; }
    public string? TypeFilter { get; set; }
}

/// <summary>
/// Individual notification item for list display
/// </summary>
public class NotificationItemViewModel
{
    public Guid Id { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
}
