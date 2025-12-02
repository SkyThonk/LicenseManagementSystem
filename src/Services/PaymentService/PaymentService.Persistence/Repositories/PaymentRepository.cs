using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Common.Interfaces.Repositories;
using PaymentService.Domain.Common.Enums;
using PaymentService.Domain.Payments;
using PaymentService.Persistence.Common.Abstractions;
using PaymentService.Persistence.Data;

namespace PaymentService.Persistence.Repositories;

/// <summary>
/// Repository for Payment entity persistence operations
/// </summary>
internal sealed class PaymentRepository : Repository<Payment, PaymentId>, IPaymentRepository
{
    public PaymentRepository(DataContext dataContext)
        : base(dataContext)
    {
    }

    public async Task<Payment?> GetByIdAsync(PaymentId id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Payment>()
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId, cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Payment>()
            .Where(p => p.TenantId == tenantId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByLicenseIdAsync(Guid licenseId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Payment>()
            .Where(p => p.LicenseId == licenseId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByApplicantIdAsync(Guid applicantId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Payment>()
            .Where(p => p.ApplicantId == applicantId && p.TenantId == tenantId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Payment>()
            .FirstOrDefaultAsync(p => p.ReferenceNumber == referenceNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetPendingPaymentsAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        return await _dataContext.Set<Payment>()
            .Where(p => p.Status == PaymentStatus.Pending)
            .OrderBy(p => p.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }
}
