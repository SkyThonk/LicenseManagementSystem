using Microsoft.AspNetCore.Mvc;
using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.Filters;

namespace LicenseManagement.Web.Controllers;

[RequireAuthentication]
[Route("notifications")]
public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet("")]
    [HttpGet("index")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, bool? unreadOnly = null)
    {
        var viewModel = await _notificationService.GetNotificationsAsync(page, pageSize, unreadOnly);
        return View(viewModel);
    }

    [HttpPost("mark-as-read/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _notificationService.MarkAsReadAsync(id);
        if (result)
        {
            TempData["Success"] = "Notification marked as read.";
        }
        else
        {
            TempData["Error"] = "Failed to mark notification as read.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("mark-all-as-read")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var result = await _notificationService.MarkAllAsReadAsync();
        if (result)
        {
            TempData["Success"] = "All notifications marked as read.";
        }
        else
        {
            TempData["Error"] = "Failed to mark all notifications as read.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _notificationService.DeleteNotificationAsync(id);
        if (result)
        {
            TempData["Success"] = "Notification deleted successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to delete notification.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var count = await _notificationService.GetUnreadCountAsync();
        return Json(new { count });
    }
}
