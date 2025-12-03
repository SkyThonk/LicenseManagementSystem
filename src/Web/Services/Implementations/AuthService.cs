using System.Text;
using System.Text.Json;
using LicenseManagement.Web.Models.Auth;
using LicenseManagement.Web.Services.Abstractions;

namespace LicenseManagement.Web.Services.Implementations;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private const string SessionKey = "UserSession";

    public AuthService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _logger = logger;

        // Configure base address for TenantService
        var tenantServiceUrl = _configuration["TenantServiceUrl"] ?? "http://localhost:5002";
        _httpClient.BaseAddress = new Uri(tenantServiceUrl);
    }

    public async Task<(bool Success, string? ErrorMessage, LoginResponse? Response)> LoginAsync(
        string email, 
        string password, 
        CancellationToken ct = default)
    {
        try
        {
            var loginRequest = new
            {
                Email = email,
                Password = password
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/authentication/login", content, ct);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(ct);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, options);

                if (loginResponse != null)
                {
                    SaveUserSession(loginResponse);
                    _logger.LogInformation("User {Email} logged in successfully", email);
                    return (true, null, loginResponse);
                }

                return (false, "Failed to parse login response", null);
            }

            var errorContent = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("Login failed for {Email}: {StatusCode} - {Error}", 
                email, response.StatusCode, errorContent);

            // Try to parse error message from response
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return (false, errorResponse?.Message ?? "Login failed", null);
            }
            catch
            {
                return (false, "Invalid email or password", null);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed during login for {Email}", email);
            return (false, "Unable to connect to authentication service", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for {Email}", email);
            return (false, "An unexpected error occurred", null);
        }
    }

    public UserSession? GetCurrentUser()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null) return null;

        var sessionData = session.GetString(SessionKey);
        if (string.IsNullOrEmpty(sessionData)) return null;

        try
        {
            var userSession = JsonSerializer.Deserialize<UserSession>(sessionData);
            if (userSession != null && !userSession.IsExpired)
            {
                return userSession;
            }

            // Session expired, clear it
            if (userSession?.IsExpired == true)
            {
                ClearUserSession();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public bool IsAuthenticated()
    {
        var user = GetCurrentUser();
        return user != null && !string.IsNullOrEmpty(user.Token);
    }

    public void SaveUserSession(LoginResponse response)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null) return;

        var userSession = new UserSession
        {
            UserId = response.UserId,
            Email = response.Email,
            FirstName = response.FirstName,
            LastName = response.LastName,
            Role = response.Role,
            Token = response.Token,
            ExpiresAt = DateTime.UtcNow.AddHours(24) // Token validity
        };

        var sessionData = JsonSerializer.Serialize(userSession);
        session.SetString(SessionKey, sessionData);
    }

    public void ClearUserSession()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        session?.Remove(SessionKey);
        session?.Clear();
    }

    public string? GetToken()
    {
        return GetCurrentUser()?.Token;
    }

    private class ErrorResponse
    {
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}
