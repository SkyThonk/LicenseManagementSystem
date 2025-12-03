using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LicenseManagement.Web.Services.Abstractions;

namespace LicenseManagement.Web.Services.Implementations;

public class DocumentApiService : IDocumentApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly ILogger<DocumentApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public DocumentApiService(HttpClient httpClient, IAuthService authService, ILogger<DocumentApiService> logger)
    {
        _httpClient = httpClient;
        _authService = authService;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    private void SetAuthorizationHeader()
    {
        var token = _authService.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
            }

            _logger.LogWarning("GET {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling GET {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<TResponse?> PostFormDataAsync<TResponse>(string endpoint, MultipartFormDataContent content, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
            }

            _logger.LogWarning("POST (form-data) {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling POST (form-data) {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<UploadDocumentApiResult?> UploadFileAsync(string endpoint, IFormFile file, Guid licenseId, string documentType, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader();
            
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            using var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
            
            content.Add(streamContent, "file", file.FileName);
            content.Add(new StringContent(licenseId.ToString()), "licenseId");
            content.Add(new StringContent(documentType), "documentType");
            
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UploadDocumentApiResult>(_jsonOptions, cancellationToken);
            }

            _logger.LogWarning("Upload file to {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to {Endpoint}", endpoint);
            return null;
        }
    }

    public async Task<TResponse?> DeleteAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
            }

            _logger.LogWarning("DELETE {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling DELETE {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling DELETE {Endpoint}", endpoint);
            return false;
        }
    }
}
