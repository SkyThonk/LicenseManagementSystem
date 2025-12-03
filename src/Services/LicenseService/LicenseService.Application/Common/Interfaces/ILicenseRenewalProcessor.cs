namespace LicenseService.Application.Common.Interfaces;

/// <summary>
/// Interface for processing license renewals in background jobs.
/// Allows the infrastructure layer to process pending renewals.
/// </summary>
public interface ILicenseRenewalProcessor
{
    /// <summary>
    /// Processes all pending license renewals for a specific tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of renewals processed</returns>
    Task<int> ProcessPendingRenewalsAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
