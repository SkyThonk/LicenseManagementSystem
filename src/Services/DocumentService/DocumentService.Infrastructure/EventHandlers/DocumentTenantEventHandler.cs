using Common.Domain.Events;
using Common.Infrastructure.Messaging;
using DocumentService.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DocumentService.Infrastructure.EventHandlers;

/// <summary>
/// Handles tenant lifecycle events for DocumentService.
/// Provisions and manages tenant-specific document database.
/// Each tenant has their own isolated database - no TenantId column needed.
/// </summary>
public class DocumentTenantEventHandler : ITenantEventHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DocumentTenantEventHandler> _logger;

    public DocumentTenantEventHandler(
        IServiceProvider serviceProvider,
        ILogger<DocumentTenantEventHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Provisions tenant-specific database when a new tenant is created.
    /// Creates and migrates the document database for the new tenant.
    /// </summary>
    public async Task HandleTenantCreatedAsync(TenantCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing TenantCreatedEvent for tenant {TenantId} ({AgencyCode})",
            @event.TenantId,
            @event.AgencyCode);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            // Ensure the database and schema exist (migrations applied)
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
            if (pendingMigrations.Any())
            {
                _logger.LogInformation(
                    "Applying {Count} pending migrations for DocumentService tenant {TenantId}",
                    pendingMigrations.Count(),
                    @event.TenantId);
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
            else
            {
                _logger.LogDebug(
                    "No pending migrations for DocumentService tenant {TenantId}",
                    @event.TenantId);
            }

            _logger.LogInformation(
                "Successfully provisioned DocumentService database for tenant {TenantId}",
                @event.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to provision DocumentService database for tenant {TenantId}",
                @event.TenantId);
            throw; // Re-throw to allow retry
        }
    }

    /// <summary>
    /// Handles tenant update events.
    /// For DocumentService, tenant updates don't require special handling
    /// as tenant metadata is stored in TenantService.
    /// </summary>
    public Task HandleTenantUpdatedAsync(TenantUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing TenantUpdatedEvent for tenant {TenantId} - no action required",
            @event.TenantId);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles tenant deletion events.
    /// Soft deletes all documents for the tenant.
    /// </summary>
    public async Task HandleTenantDeletedAsync(TenantDeletedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing TenantDeletedEvent for tenant {TenantId}",
            @event.TenantId);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            // Since each tenant has their own database, we soft delete all documents
            // Note: In a per-tenant database model, we might consider dropping the database entirely
            // For now, we soft delete all documents
            var documents = await dbContext.Documents
                .IgnoreQueryFilters() // Include already deleted
                .Where(d => !d.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var document in documents)
            {
                document.IsDeleted = true;
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Soft deleted {DocumentCount} documents for tenant {TenantId}",
                documents.Count,
                @event.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to clean up DocumentService resources for tenant {TenantId}",
                @event.TenantId);
            throw;
        }
    }
}
