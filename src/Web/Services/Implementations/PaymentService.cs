using LicenseManagement.Web.Constants;
using LicenseManagement.Web.Services.Abstractions;
using LicenseManagement.Web.ViewModels.Payments;
using LicenseManagement.Web.ViewModels.Shared;

namespace LicenseManagement.Web.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IPaymentApiService _apiService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IPaymentApiService apiService, ILogger<PaymentService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<PaymentListViewModel> GetPaymentsAsync(int page = 1, int pageSize = 10, string? search = null, string? status = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = $"?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(status))
            {
                queryParams += $"&status={Uri.EscapeDataString(status)}";
            }

            var response = await _apiService.GetAsync<GetPaymentsApiResponse>(
                $"{ApiEndpoints.Payment.GetAll}{queryParams}",
                cancellationToken);

            if (response != null)
            {
                var items = response.Payments.Select(MapToListItemViewModel).ToList();
                return new PaymentListViewModel
                {
                    Payments = new PaginatedList<PaymentItemViewModel>(
                        items,
                        response.TotalCount,
                        response.Page,
                        response.PageSize),
                    SearchQuery = search,
                    StatusFilter = status
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching payments list");
        }

        return new PaymentListViewModel
        {
            Payments = new PaginatedList<PaymentItemViewModel>([], 0, page, pageSize),
            SearchQuery = search,
            StatusFilter = status
        };
    }

    public async Task<PaymentDetailsViewModel?> GetPaymentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = string.Format(ApiEndpoints.Payment.GetById, id);
            var response = await _apiService.GetAsync<GetPaymentDetailApiResponse>(endpoint, cancellationToken);

            if (response != null)
            {
                return MapDetailResponseToViewModel(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching payment {PaymentId}", id);
        }

        return null;
    }

    public async Task<bool> RefundPaymentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new RefundPaymentApiRequest { PaymentId = id };
            var endpoint = $"{ApiEndpoints.Payment.GetAll}/{id}/refund";
            var response = await _apiService.PostAsync<RefundPaymentApiRequest, RefundPaymentApiResponse>(
                endpoint, request, cancellationToken);
            return response?.Success ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment {PaymentId}", id);
            return false;
        }
    }

    private static PaymentItemViewModel MapToListItemViewModel(PaymentApiDto dto)
    {
        return new PaymentItemViewModel
        {
            Id = dto.PaymentId,
            ReferenceNumber = dto.ReferenceNumber ?? $"PAY-{dto.PaymentId.ToString()[..8].ToUpper()}",
            Amount = dto.Amount,
            Currency = dto.Currency,
            Status = dto.Status,
            CreatedAt = dto.CreatedAt,
            PaidAt = dto.PaidAt,
            TenantName = "Current Tenant",
            LicenseNumber = dto.LicenseId.ToString()[..8].ToUpper()
        };
    }

    private static PaymentDetailsViewModel MapDetailResponseToViewModel(GetPaymentDetailApiResponse dto)
    {
        return new PaymentDetailsViewModel
        {
            Id = dto.PaymentId,
            ReferenceNumber = dto.ReferenceNumber ?? $"PAY-{dto.PaymentId.ToString()[..8].ToUpper()}",
            Amount = dto.Amount,
            Currency = dto.Currency,
            Status = dto.Status,
            CreatedAt = dto.CreatedAt,
            PaidAt = dto.PaidAt,
            TenantName = "Current Tenant",
            TenantId = dto.TenantId,
            LicenseNumber = dto.LicenseId.ToString()[..8].ToUpper(),
            LicenseId = dto.LicenseId,
            ApplicantName = "Applicant"
        };
    }
}

#region API DTOs

internal class GetPaymentsApiResponse
{
    public List<PaymentApiDto> Payments { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

internal class PaymentApiDto
{
    public Guid PaymentId { get; set; }
    public Guid LicenseId { get; set; }
    public Guid ApplicantId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? ReferenceNumber { get; set; }
}

internal class GetPaymentDetailApiResponse
{
    public Guid PaymentId { get; set; }
    public Guid TenantId { get; set; }
    public Guid LicenseId { get; set; }
    public Guid ApplicantId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ErrorMessage { get; set; }
}

internal class RefundPaymentApiRequest
{
    public Guid PaymentId { get; set; }
}

internal class RefundPaymentApiResponse
{
    public bool Success { get; set; }
    public Guid PaymentId { get; set; }
    public string? Message { get; set; }
}

#endregion
