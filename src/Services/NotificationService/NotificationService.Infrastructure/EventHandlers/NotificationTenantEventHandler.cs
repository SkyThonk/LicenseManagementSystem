using Common.Domain.Events;
using Common.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.Persistence.Data;
using NotificationService.Domain.Templates;
using NotificationService.Domain.Common.Enums;

namespace NotificationService.Infrastructure.EventHandlers;

/// <summary>
/// Handles tenant lifecycle events for NotificationService.
/// Provisions and manages tenant-specific notification resources.
/// </summary>
public class NotificationTenantEventHandler : ITenantEventHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationTenantEventHandler> _logger;

    public NotificationTenantEventHandler(
        IServiceProvider serviceProvider,
        ILogger<NotificationTenantEventHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Provisions tenant-specific resources when a new tenant is created.
    /// Creates default notification templates for the new tenant.
    /// </summary>
    public async Task HandleTenantCreatedAsync(TenantCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing TenantCreatedEvent for tenant {TenantId} ({AgencyCode})",
            @event.TenantId,
            @event.AgencyCode);

        try
        {
            // Create a scope for the DbContext
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            // Ensure the database and schema exist (migrations applied)
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Applying {Count} pending migrations for NotificationService", pendingMigrations.Count());
                await dbContext.Database.MigrateAsync(cancellationToken);
            }

            // Seed default notification templates for the new tenant
            await SeedDefaultTemplatesAsync(dbContext, @event.TenantId, cancellationToken);

            _logger.LogInformation(
                "Successfully provisioned NotificationService resources for tenant {TenantId}",
                @event.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to provision NotificationService resources for tenant {TenantId}",
                @event.TenantId);
            throw; // Re-throw to allow retry
        }
    }

    public async Task HandleTenantUpdatedAsync(TenantUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing TenantUpdatedEvent for tenant {TenantId}",
            @event.TenantId);

        // For NotificationService, tenant updates don't require special handling
        // The tenant metadata is stored in TenantService; we only reference by ID
        await Task.CompletedTask;
    }

    public async Task HandleTenantDeletedAsync(TenantDeletedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing TenantDeletedEvent for tenant {TenantId}",
            @event.TenantId);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            // Soft delete all notifications for the tenant
            var notifications = await dbContext.Notifications
                .Where(n => n.TenantId == @event.TenantId && !n.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var notification in notifications)
            {
                notification.IsDeleted = true;
            }

            // Soft delete all templates for the tenant
            var templates = await dbContext.NotificationTemplates
                .Where(t => t.TenantId == @event.TenantId && !t.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var template in templates)
            {
                template.IsDeleted = true;
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Soft deleted {NotificationCount} notifications and {TemplateCount} templates for tenant {TenantId}",
                notifications.Count,
                templates.Count,
                @event.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to clean up NotificationService resources for tenant {TenantId}",
                @event.TenantId);
            throw;
        }
    }

    /// <summary>
    /// Seeds default notification templates for a new tenant.
    /// </summary>
    private async Task SeedDefaultTemplatesAsync(DataContext dbContext, Guid tenantId, CancellationToken cancellationToken)
    {
        // Check if tenant already has templates (idempotency)
        var existingTemplates = await dbContext.NotificationTemplates
            .AnyAsync(t => t.TenantId == tenantId, cancellationToken);

        if (existingTemplates)
        {
            _logger.LogDebug("Tenant {TenantId} already has notification templates, skipping seed", tenantId);
            return;
        }

        // Seed common default notification templates
        var defaultTemplates = new[]
        {
            NotificationTemplate.Create(
                tenantId: tenantId,
                templateName: "License Approved",
                subject: "Your License Application Has Been Approved",
                body: "Dear {ApplicantName},\n\nWe are pleased to inform you that your license application (Reference: {LicenseNumber}) has been approved.\n\nYour license is valid until {ExpiryDate}.\n\nThank you for your application.\n\nBest regards,\n{AgencyName}",
                notificationType: NotificationType.Email
            ),
            NotificationTemplate.Create(
                tenantId: tenantId,
                templateName: "License Rejected",
                subject: "Your License Application Has Been Rejected",
                body: "Dear {ApplicantName},\n\nWe regret to inform you that your license application (Reference: {ReferenceNumber}) has been rejected.\n\nReason: {RejectionReason}\n\nIf you have any questions, please contact our office.\n\nBest regards,\n{AgencyName}",
                notificationType: NotificationType.Email
            ),
            NotificationTemplate.Create(
                tenantId: tenantId,
                templateName: "Renewal Reminder",
                subject: "License Renewal Reminder - {LicenseNumber}",
                body: "Dear {ApplicantName},\n\nThis is a reminder that your license ({LicenseNumber}) will expire on {ExpiryDate}.\n\nPlease submit your renewal application before the expiry date to avoid any disruption.\n\nThank you.\n\nBest regards,\n{AgencyName}",
                notificationType: NotificationType.Email
            ),
            NotificationTemplate.Create(
                tenantId: tenantId,
                templateName: "Payment Received",
                subject: "Payment Confirmation - {PaymentReference}",
                body: "Dear {ApplicantName},\n\nWe have received your payment of {PaymentAmount} for {PaymentDescription}.\n\nPayment Reference: {PaymentReference}\nDate: {PaymentDate}\n\nThank you for your payment.\n\nBest regards,\n{AgencyName}",
                notificationType: NotificationType.Email
            ),
            NotificationTemplate.Create(
                tenantId: tenantId,
                templateName: "SMS Renewal Reminder",
                subject: "Renewal Reminder",
                body: "Your license {LicenseNumber} expires on {ExpiryDate}. Please renew before expiry.",
                notificationType: NotificationType.SMS
            )
        };

        dbContext.NotificationTemplates.AddRange(defaultTemplates);
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Seeded {Count} default notification templates for tenant {TenantId}",
            defaultTemplates.Length,
            tenantId);
    }
}
