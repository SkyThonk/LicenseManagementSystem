namespace LicenseManagement.Web.Services.Abstractions;

/// <summary>
/// HTTP client service for NotificationService API (port 5005)
/// </summary>
public interface INotificationApiService
{
    Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);
    Task<TResponse?> PatchAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);
    Task<TResponse?> DeleteAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);
}
