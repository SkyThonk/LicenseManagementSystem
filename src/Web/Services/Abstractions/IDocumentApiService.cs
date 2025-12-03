namespace LicenseManagement.Web.Services.Abstractions;

/// <summary>
/// HTTP client service for DocumentService API (port 5004)
/// </summary>
public interface IDocumentApiService
{
    Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);
    Task<TResponse?> PostFormDataAsync<TResponse>(string endpoint, MultipartFormDataContent content, CancellationToken cancellationToken = default);
    Task<UploadDocumentApiResult?> UploadFileAsync(string endpoint, IFormFile file, Guid licenseId, string documentType, CancellationToken cancellationToken = default);
    Task<TResponse?> DeleteAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
}

public class UploadDocumentApiResult
{
    public Guid Id { get; set; }
    public string? FileName { get; set; }
    public DateTime UploadedAt { get; set; }
}
