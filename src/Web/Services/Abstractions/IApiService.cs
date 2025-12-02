namespace LicenseManagement.Web.Services.Abstractions;

public interface IApiService
{
    Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);
    Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
    Task<TResponse?> PostFormDataAsync<TResponse>(string endpoint, MultipartFormDataContent content, CancellationToken cancellationToken = default);
}
