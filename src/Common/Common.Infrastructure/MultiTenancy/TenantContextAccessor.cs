using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Common.Infrastructure.MultiTenancy;

/// <summary>
/// Resolves tenant context from JWT claims in HTTP requests.
/// Each authenticated request contains a TenantId claim that determines which database to use.
/// </summary>
public class TenantContextAccessor : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TenantDatabaseSettings _settings;
    private Guid? _cachedTenantId;
    private string? _cachedConnectionString;

    public TenantContextAccessor(
        IHttpContextAccessor httpContextAccessor,
        IOptions<TenantDatabaseSettings> settings)
    {
        _httpContextAccessor = httpContextAccessor;
        _settings = settings.Value;
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

            if (!_settings.UseSeparateDatabases)
            {
                _cachedConnectionString = _settings.CentralConnectionString;
                return _cachedConnectionString;
            }

            // Replace placeholder with actual tenant ID
            _cachedConnectionString = _settings.TenantConnectionStringTemplate
                .Replace("{TenantId}", TenantId.ToString())
                .Replace("{tenantId}", TenantId.ToString());

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
