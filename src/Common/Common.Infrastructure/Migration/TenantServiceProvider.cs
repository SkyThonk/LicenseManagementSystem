using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.Migration;

/// <summary>
/// Tenant provider that fetches tenants from TenantService API
/// </summary>
public class TenantServiceProvider : ITenantProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantServiceProvider> _logger;
    private readonly string _tenantServiceUrl;

    public TenantServiceProvider(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<TenantServiceProvider> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _tenantServiceUrl = configuration["TenantServiceUrl"] ?? "http://localhost:5002";
    }

    public async Task<IEnumerable<TenantInfo>> GetAllTenantsAsync(CancellationToken ct = default)
    {
        try
        {
            var url = $"{_tenantServiceUrl}/api/tenant/migration/all";
            _logger.LogInformation("Fetching tenants from {Url}", url);

            var response = await _httpClient.GetAsync(url, ct);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch tenants. Status: {StatusCode}", response.StatusCode);
                return Enumerable.Empty<TenantInfo>();
            }

            var tenants = await response.Content.ReadFromJsonAsync<IEnumerable<TenantMigrationDto>>(ct);
            
            if (tenants == null)
            {
                _logger.LogWarning("No tenants returned from TenantService");
                return Enumerable.Empty<TenantInfo>();
            }

            return tenants.Select(t => new TenantInfo(
                t.Id,
                t.Name,
                t.ConnectionString ?? GetDefaultConnectionString(t.Id)
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tenants from TenantService");
            return Enumerable.Empty<TenantInfo>();
        }
    }

    private string GetDefaultConnectionString(Guid tenantId)
    {
        // Use the default connection string pattern if tenant doesn't have a specific one
        var baseConnectionString = _configuration.GetConnectionString("SQL") ?? "";
        return baseConnectionString;
    }

    private record TenantMigrationDto(
        Guid Id,
        string Name,
        string? ConnectionString
    );
}
