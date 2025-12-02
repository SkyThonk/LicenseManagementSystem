using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.ViewModels.Payments;
using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IApiService _apiService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IApiService apiService, ILogger<PaymentService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public Task<PaymentListViewModel> GetPaymentsAsync(int page = 1, int pageSize = 10, string? search = null, string? status = null, CancellationToken cancellationToken = default)
    {
        // TODO: Call actual API
        var payments = new List<PaymentItemViewModel>
        {
            new PaymentItemViewModel
            {
                Id = Guid.NewGuid(),
                ReferenceNumber = "PAY-2024-001",
                Amount = 299.00m,
                Currency = "USD",
                Status = "Completed",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                PaidAt = DateTime.UtcNow.AddDays(-1),
                TenantName = "Acme Corporation",
                LicenseNumber = "LIC-2024-001"
            },
            new PaymentItemViewModel
            {
                Id = Guid.NewGuid(),
                ReferenceNumber = "PAY-2024-002",
                Amount = 199.00m,
                Currency = "USD",
                Status = "Completed",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                PaidAt = DateTime.UtcNow.AddDays(-2),
                TenantName = "Tech Solutions Inc.",
                LicenseNumber = "LIC-2024-002"
            },
            new PaymentItemViewModel
            {
                Id = Guid.NewGuid(),
                ReferenceNumber = "PAY-2024-003",
                Amount = 99.00m,
                Currency = "USD",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                PaidAt = null,
                TenantName = "StartUp Co.",
                LicenseNumber = "LIC-2024-004"
            },
            new PaymentItemViewModel
            {
                Id = Guid.NewGuid(),
                ReferenceNumber = "PAY-2024-004",
                Amount = 149.00m,
                Currency = "USD",
                Status = "Failed",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                PaidAt = null,
                TenantName = "Global Services Ltd.",
                LicenseNumber = "LIC-2024-003"
            }
        };

        return Task.FromResult(new PaymentListViewModel
        {
            Payments = new PaginatedList<PaymentItemViewModel>(payments, 4, page, pageSize),
            SearchQuery = search,
            StatusFilter = status
        });
    }

    public Task<PaymentDetailsViewModel?> GetPaymentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // TODO: Call actual API
        return Task.FromResult<PaymentDetailsViewModel?>(new PaymentDetailsViewModel
        {
            Id = id,
            ReferenceNumber = "PAY-2024-001",
            Amount = 299.00m,
            Currency = "USD",
            Status = "Completed",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            PaidAt = DateTime.UtcNow.AddDays(-1),
            TenantName = "Acme Corporation",
            TenantId = Guid.NewGuid(),
            LicenseNumber = "LIC-2024-001",
            LicenseId = Guid.NewGuid(),
            ApplicantName = "John Smith"
        });
    }

    public Task<bool> RefundPaymentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Call actual API
            _logger.LogInformation("Refunding payment: {Id}", id);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment");
            return Task.FromResult(false);
        }
    }
}
