using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LicenseService.Infrastructure.BackgroundJobs;

/// <summary>
/// Configuration settings for the license renewal background job.
/// </summary>
public class LicenseRenewalJobSettings
{
    public const string SectionName = "LicenseRenewalJob";

    /// <summary>
    /// The interval in minutes between renewal processing runs.
    /// Default is 60 minutes (1 hour).
    /// </summary>
    public int IntervalMinutes { get; set; } = 60;

    /// <summary>
    /// Whether the background job is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// The delay in seconds before the first run after startup.
    /// Allows the application to fully initialize.
    /// </summary>
    public int InitialDelaySeconds { get; set; } = 30;
}

/// <summary>
/// Background service that periodically processes license renewals.
/// This job runs on a timer and processes all pending renewals across tenants.
/// </summary>
public class LicenseRenewalBackgroundJob : BackgroundService
{
    private readonly ILogger<LicenseRenewalBackgroundJob> _logger;
    private readonly LicenseRenewalJobSettings _settings;

    public LicenseRenewalBackgroundJob(
        ILogger<LicenseRenewalBackgroundJob> logger,
        IOptions<LicenseRenewalJobSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("License renewal background job is disabled");
            return;
        }

        _logger.LogInformation(
            "License renewal background job started. Will run every {IntervalMinutes} minutes",
            _settings.IntervalMinutes);

        // Initial delay to allow the application to fully start
        await Task.Delay(TimeSpan.FromSeconds(_settings.InitialDelaySeconds), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessRenewalsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Graceful shutdown, don't log as error
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing license renewals");
            }

            // Wait for the next interval
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(_settings.IntervalMinutes), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("License renewal background job stopped");
    }

    private async Task ProcessRenewalsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting license renewal processing job at {Time}", DateTime.UtcNow);

        // TODO: Implement the actual renewal processing logic
        // This should:
        // 1. Get list of active tenants (from TenantService or cached list)
        // 2. For each tenant, create a scoped service provider
        // 3. Get pending renewals for the tenant
        // 4. Process each renewal (check payment status, update license expiry, send notifications)
        //
        // Example implementation structure:
        // 
        // var tenants = await _tenantService.GetAllActiveTenantsAsync(cancellationToken);
        // foreach (var tenant in tenants)
        // {
        //     using var scope = _serviceProvider.CreateScope();
        //     var renewalProcessor = scope.ServiceProvider.GetRequiredService<ILicenseRenewalProcessor>();
        //     var processedCount = await renewalProcessor.ProcessPendingRenewalsAsync(tenant.Id, cancellationToken);
        //     _logger.LogInformation("Processed {Count} renewals for tenant {TenantId}", processedCount, tenant.Id);
        // }

        await Task.CompletedTask;

        _logger.LogInformation("Completed license renewal processing job at {Time}", DateTime.UtcNow);
    }
}
