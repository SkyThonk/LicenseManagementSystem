using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Common.Infrastructure.MultiTenancy;

/// <summary>
/// Resolves tenant context from JWT claims in HTTP requests.
/// Each authenticated request contains a TenantId claim that determines which database to use.
/// </summary>
public class TenantContextAccessor : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly TenantDatabaseSettings _settings;
    private readonly string _servicePrefix;
    private Guid? _cachedTenantId;
    private string? _cachedConnectionString;

    public TenantContextAccessor(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IOptions<TenantDatabaseSettings> settings)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _settings = settings.Value;
        _servicePrefix = _settings.ServicePrefix ?? "service";
    }

    public Guid TenantId
    {
        get
        {
            if (_cachedTenantId.HasValue)
                return _cachedTenantId.Value;

            var tenantIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenantId")
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GroupSid);

            if (tenantIdClaim != null && Guid.TryParse(tenantIdClaim.Value, out var tenantId))
            {
                _cachedTenantId = tenantId;
                return tenantId;
            }

            throw new InvalidOperationException("TenantId claim not found in JWT token. Ensure the user is authenticated and has a valid tenant.");
        }
    }

    public string ConnectionString
    {
        get
        {
            if (_cachedConnectionString != null)
                return _cachedConnectionString;

            // Get the base connection string
            var baseConnectionString = _configuration.GetConnectionString("SQL") 
                ?? _settings.CentralConnectionString;

            if (string.IsNullOrEmpty(baseConnectionString))
            {
                throw new InvalidOperationException("SQL connection string not configured");
            }

            // Build tenant-specific database name: {servicePrefix}_{tenantId}
            var tenantDbName = $"{_servicePrefix}_{TenantId:N}".ToLowerInvariant();

            // Parse and modify the connection string
            var builder = new NpgsqlConnectionStringBuilder(baseConnectionString)
            {
                Database = tenantDbName
            };

            _cachedConnectionString = builder.ConnectionString;
            return _cachedConnectionString;
        }
    }

    public bool IsAvailable
    {
        get
        {
            try
            {
                var tenantIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("tenantId")
                    ?? _httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")
                    ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GroupSid);

                return tenantIdClaim != null && Guid.TryParse(tenantIdClaim.Value, out _);
            }
            catch
            {
                return false;
            }
        }
    }
}
