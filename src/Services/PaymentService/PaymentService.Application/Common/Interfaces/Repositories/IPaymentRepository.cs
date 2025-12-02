using PaymentService.Domain.Payments;

namespace PaymentService.Application.Common.Interfaces.Repositories;

/// <summary>
/// Repository interface for Payment entity
/// </summary>
public interface IPaymentRepository
{
    void Add(Payment payment);
    void Update(Payment payment);
    Task<Payment?> GetByIdAsync(PaymentId id, CancellationToken cancellationToken = default);
    Task<Payment?> GetByIdAsync(PaymentId id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByLicenseIdAsync(Guid licenseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByApplicantIdAsync(Guid applicantId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetPendingPaymentsAsync(int batchSize = 100, CancellationToken cancellationToken = default);
}
