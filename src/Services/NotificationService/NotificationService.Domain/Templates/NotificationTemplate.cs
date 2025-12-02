using Common.Domain.Abstractions;
using NotificationService.Domain.Common.Enums;

namespace NotificationService.Domain.Templates;

/// <summary>
/// Represents a notification template for reusable content.
/// Tenants can define their own templates like "License Approved", "Renewal Reminder", etc.
/// </summary>
public sealed class NotificationTemplate : Entity<NotificationTemplateId>
{
    private NotificationTemplate(
        NotificationTemplateId id,
        Guid tenantId,
        string templateName,
        string subject,
        string body,
        NotificationType notificationType,
        Guid? createdBy = null
    ) : base(id, createdBy)
    {
        TenantId = tenantId;
        TemplateName = templateName;
        Subject = subject;
        Body = body;
        NotificationType = notificationType;
        IsActive = true;
    }

    // For EF Core
    private NotificationTemplate() { }

    /// <summary>
    /// The tenant (government agency) that owns this template
    /// </summary>
    public Guid TenantId { get; private set; }

    /// <summary>
    /// Name of the template (e.g., "License Approved", "Renewal Reminder")
    /// </summary>
    public string TemplateName { get; private set; } = null!;

    /// <summary>
    /// Subject template (supports placeholders like {ApplicantName})
    /// </summary>
    public string Subject { get; private set; } = null!;

    /// <summary>
    /// Body template (supports placeholders like {LicenseNumber}, {ExpiryDate})
    /// </summary>
    public string Body { get; private set; } = null!;

    /// <summary>
    /// Type of notification this template is for
    /// </summary>
    public NotificationType NotificationType { get; private set; }

    /// <summary>
    /// Whether the template is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Creates a new notification template
    /// </summary>
    public static NotificationTemplate Create(
        Guid tenantId,
        string templateName,
        string subject,
        string body,
        NotificationType notificationType,
        Guid? createdBy = null)
    {
        if (string.IsNullOrWhiteSpace(templateName))
            throw new ArgumentException("Template name is required.", nameof(templateName));

        if (string.IsNullOrWhiteSpace(subject))
            throw new ArgumentException("Subject is required.", nameof(subject));

        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Body is required.", nameof(body));

        return new NotificationTemplate(
            NotificationTemplateId.New(),
            tenantId,
            templateName,
            subject,
            body,
            notificationType,
            createdBy);
    }

    /// <summary>
    /// Updates the template
    /// </summary>
    public void Update(
        string? templateName = null,
        string? subject = null,
        string? body = null,
        NotificationType? notificationType = null,
        Guid? updatedBy = null)
    {
        if (!string.IsNullOrWhiteSpace(templateName))
            TemplateName = templateName;

        if (!string.IsNullOrWhiteSpace(subject))
            Subject = subject;

        if (!string.IsNullOrWhiteSpace(body))
            Body = body;

        if (notificationType.HasValue)
            NotificationType = notificationType.Value;

        SetUpdatedBy(updatedBy);
    }

    /// <summary>
    /// Activates the template
    /// </summary>
    public void Activate(Guid? updatedBy = null)
    {
        IsActive = true;
        SetUpdatedBy(updatedBy);
    }

    /// <summary>
    /// Deactivates the template
    /// </summary>
    public void Deactivate(Guid? updatedBy = null)
    {
        IsActive = false;
        SetUpdatedBy(updatedBy);
    }

    /// <summary>
    /// Renders the template with provided placeholders
    /// </summary>
    public (string Subject, string Body) Render(Dictionary<string, string> placeholders)
    {
        var renderedSubject = Subject;
        var renderedBody = Body;

        foreach (var placeholder in placeholders)
        {
            renderedSubject = renderedSubject.Replace($"{{{placeholder.Key}}}", placeholder.Value);
            renderedBody = renderedBody.Replace($"{{{placeholder.Key}}}", placeholder.Value);
        }

        return (renderedSubject, renderedBody);
    }
}
