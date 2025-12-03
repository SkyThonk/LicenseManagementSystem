using Common.Domain.Events;
using Common.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LicenseService.Persistence.Data;

namespace LicenseService.Infrastructure.EventHandlers;

/// <summary>
/// Handles tenant lifecycle events for LicenseService.
/// Provisions and manages tenant-specific database resources.
/// With separate databases per tenant, this handler manages database-level operations.
/// </summary>
public class LicenseTenantEventHandler : ITenantEventHandler
{
    private readonly DataContext _dbContext;
    private readonly ILogger<LicenseTenantEventHandler> _logger;

    public LicenseTenantEventHandler(
        DataContext dbContext,
        ILogger<LicenseTenantEventHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Provisions tenant-specific resources when a new tenant is created.
    /// With separate databases per tenant, this ensures the database schema is up-to-date
    /// and optionally seeds default data for the tenant's database.
    /// </summary>
    public async Task HandleTenantCreatedAsync(TenantCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing TenantCreatedEvent for tenant {TenantId} ({AgencyCode})",
            @event.TenantId,
            @event.AgencyCode);

        try
        {
            // Ensure the database and schema exist (migrations applied)
            // In production, migrations should be applied during deployment,
            // but this ensures the schema exists for the tenant's data
            var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Applying {Count} pending migrations for LicenseService", pendingMigrations.Count());
                await _dbContext.Database.MigrateAsync(cancellationToken);
            }

            // Optionally seed default license types for the new tenant's database
            await SeedDefaultLicenseTypesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully provisioned LicenseService resources for tenant {TenantId}",
                @event.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to provision LicenseService resources for tenant {TenantId}",
                @event.TenantId);
            throw; // Re-throw to allow retry
        }
    }

    public async Task HandleTenantUpdatedAsync(TenantUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing TenantUpdatedEvent for tenant {TenantId}",
            @event.TenantId);

        // For LicenseService, tenant updates don't require special handling
        // The tenant metadata is stored in TenantService; we only reference by ID
        await Task.CompletedTask;
    }

    public async Task HandleTenantDeletedAsync(TenantDeletedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing TenantDeletedEvent for tenant {TenantId}",
            @event.TenantId);

        // With separate databases per tenant, soft delete all entities in the tenant's database
        // In a real scenario, you might archive data or move to cold storage
        var licenses = await _dbContext.Licenses
            .Where(l => !l.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var license in licenses)
        {
            license.IsDeleted = true;
        }

        var licenseTypes = await _dbContext.LicenseTypes
            .Where(lt => !lt.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var licenseType in licenseTypes)
        {
            licenseType.IsDeleted = true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Soft deleted {LicenseCount} licenses and {TypeCount} license types for tenant {TenantId}",
            licenses.Count,
            licenseTypes.Count,
            @event.TenantId);
    }

    /// <summary>
    /// Seeds default license types for a new tenant's database.
    /// </summary>
    private async Task SeedDefaultLicenseTypesAsync(CancellationToken cancellationToken)
    {
        // Check if tenant already has license types (idempotency)
        var existingTypes = await _dbContext.LicenseTypes.AnyAsync(cancellationToken);

        if (existingTypes)
        {
            _logger.LogDebug("Tenant database already has license types, skipping seed");
            return;
        }

        // Seed some common default license types
        // The tenant admin can customize these later
        var defaultTypes = new[]
        {
            Domain.LicenseTypes.LicenseType.Create(
                name: "Professional License",
                description: "Standard professional license for practitioners",
                feeAmount: 150.00m
            ),
            Domain.LicenseTypes.LicenseType.Create(
                name: "Business License",
                description: "License for business operations",
                feeAmount: 250.00m
            ),
            Domain.LicenseTypes.LicenseType.Create(
                name: "Temporary Permit",
                description: "Short-term permit for temporary activities",
                feeAmount: 50.00m
            )
        };

        _dbContext.LicenseTypes.AddRange(defaultTypes);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Seeded {Count} default license types for tenant database",
            defaultTypes.Length);
    }
}
