using Common.Domain.Abstractions;
using NotificationService.Domain.Common.Enums;

namespace NotificationService.Domain.Notifications;

/// <summary>
/// Represents a notification record (email, SMS, push notification).
/// Core entity of the NotificationService.
/// </summary>
public sealed class Notification : Entity<NotificationId>
{
    private Notification(
        NotificationId id,
        Guid tenantId,
        string recipient,
        string? subject,
        string message,
        NotificationType notificationType,
        NotificationStatus status,
        Guid? createdBy = null
    ) : base(id, createdBy)
    {
        TenantId = tenantId;
        Recipient = recipient;
        Subject = subject;
        Message = message;
        NotificationType = notificationType;
        Status = status;
        SentAt = null;
    }

    // For EF Core
    private Notification() { }

    /// <summary>
    /// The tenant (government agency) that owns this notification
    /// </summary>
    public Guid TenantId { get; private set; }

    /// <summary>
    /// Recipient address (email or mobile number)
    /// </summary>
    public string Recipient { get; private set; } = null!;

    /// <summary>
    /// Subject line (optional for SMS)
    /// </summary>
    public string? Subject { get; private set; }

    /// <summary>
    /// Notification body content
    /// </summary>
    public string Message { get; private set; } = null!;

    /// <summary>
    /// Type of notification (Email, SMS, Push)
    /// </summary>
    public NotificationType NotificationType { get; private set; }

    /// <summary>
    /// Current status of the notification
    /// </summary>
    public NotificationStatus Status { get; private set; }

    /// <summary>
    /// When the notification was sent (null if not sent yet)
    /// </summary>
    public DateTime? SentAt { get; private set; }

    /// <summary>
    /// Error message if sending failed
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Reference to template used (if any)
    /// </summary>
    public Guid? TemplateId { get; private set; }

    /// <summary>
    /// Creates a new notification
    /// </summary>
    public static Notification Create(
        Guid tenantId,
        string recipient,
        string message,
        NotificationType notificationType,
        string? subject = null,
        Guid? templateId = null,
        Guid? createdBy = null)
    {
        if (string.IsNullOrWhiteSpace(recipient))
            throw new ArgumentException("Recipient is required.", nameof(recipient));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message is required.", nameof(message));

        var notification = new Notification(
            NotificationId.New(),
            tenantId,
            recipient,
            subject,
            message,
            notificationType,
            NotificationStatus.Pending,
            createdBy)
        {
            TemplateId = templateId
        };

        return notification;
    }

    /// <summary>
    /// Marks the notification as sent
    /// </summary>
    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    /// <summary>
    /// Marks the notification as failed
    /// </summary>
    public void MarkAsFailed(string errorMessage)
    {
        Status = NotificationStatus.Failed;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Marks the notification as cancelled
    /// </summary>
    public void Cancel()
    {
        if (Status == NotificationStatus.Sent)
            throw new InvalidOperationException("Cannot cancel a sent notification.");

        Status = NotificationStatus.Cancelled;
    }

    /// <summary>
    /// Retry a failed notification
    /// </summary>
    public void Retry()
    {
        if (Status != NotificationStatus.Failed)
            throw new InvalidOperationException("Can only retry failed notifications.");

        Status = NotificationStatus.Pending;
        ErrorMessage = null;
    }
}
