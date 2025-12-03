using Common.Domain.Events;
using Common.Infrastructure.Messaging;
using Common.Infrastructure.Migration;
using LicenseService.Persistence.Data;
using Microsoft.Extensions.Logging;

namespace LicenseService.Infrastructure.EventHandlers;

/// <summary>
/// Handles tenant lifecycle events for LicenseService.
/// Creates tenant-specific databases when tenants are created.
/// </summary>
public class LicenseTenantEventHandler : ITenantEventHandler
{
    private readonly ITenantDatabaseCreator _databaseCreator;
    private readonly ILogger<LicenseTenantEventHandler> _logger;

    public LicenseTenantEventHandler(
        ITenantDatabaseCreator databaseCreator,
        ILogger<LicenseTenantEventHandler> logger)
    {
        _databaseCreator = databaseCreator;
        _logger = logger;
    }

    /// <summary>
    /// Handles tenant creation event by creating a new database for the tenant.
    /// </summary>
    public async Task HandleTenantCreatedAsync(TenantCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing TenantCreatedEvent for tenant {TenantId} ({AgencyCode})",
            @event.TenantId,
            @event.AgencyCode);

        try
        {
            var connectionString = await _databaseCreator.CreateTenantDatabaseAsync<DataContext>(
                @event.TenantId,
                "license",
                cancellationToken);

            _logger.LogInformation(
                "Successfully created license database for tenant {TenantId}",
                @event.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create license database for tenant {TenantId}",
                @event.TenantId);
            throw;
        }
    }

    public Task HandleTenantUpdatedAsync(TenantUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Received TenantUpdatedEvent for tenant {TenantId}",
            @event.TenantId);

        return Task.CompletedTask;
    }

    public Task HandleTenantDeletedAsync(TenantDeletedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Received TenantDeletedEvent for tenant {TenantId}",
            @event.TenantId);

        // TODO: Consider archiving or dropping the tenant database
        return Task.CompletedTask;
    }
}
