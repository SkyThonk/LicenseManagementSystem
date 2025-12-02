using LicenseManagement.Web.ViewModels.Payments;

namespace LicenseManagement.Web.Services.Abstractions;

public interface IPaymentService
{
    Task<PaymentListViewModel> GetPaymentsAsync(int page = 1, int pageSize = 10, string? search = null, string? status = null, CancellationToken cancellationToken = default);
    Task<PaymentDetailsViewModel?> GetPaymentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> RefundPaymentAsync(Guid id, CancellationToken cancellationToken = default);
}
