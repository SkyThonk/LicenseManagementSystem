using Common.Application.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Api.Extensions;
using NotificationService.Contracts.Notifications.GetNotification;
using NotificationService.Contracts.Notifications.GetNotifications;
using NotificationService.Contracts.Notifications.SendNotification;
using Wolverine;

namespace NotificationService.Api.Controllers;

/// <summary>
/// API Controller for managing notifications
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public NotificationsController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Send a new notification
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<SendNotificationResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    [HttpGet("{notificationId:guid}")]
    public async Task<IActionResult> GetNotification([FromRoute] Guid notificationId, CancellationToken ct)
    {
        var request = new GetNotificationRequest(notificationId);
        var result = await _messageBus.InvokeAsync<Result<GetNotificationResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get paginated list of notifications for the current tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] GetNotificationsRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<GetNotificationsResponse>>(request, ct);
        return result.ToActionResult(this);
    }
}
