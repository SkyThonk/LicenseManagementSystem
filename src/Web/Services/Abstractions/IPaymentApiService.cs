namespace LicenseManagement.Web.Services.Abstractions;

/// <summary>
/// HTTP client service for PaymentService API (port 5006)
/// </summary>
public interface IPaymentApiService
{
    Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
}
