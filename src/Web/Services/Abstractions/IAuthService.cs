using LicenseManagement.Web.Models.Auth;

namespace LicenseManagement.Web.Services.Abstractions;

/// <summary>
/// Authentication service for handling login/logout with TenantService
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Login with email and password
    /// </summary>
    Task<(bool Success, string? ErrorMessage, LoginResponse? Response)> LoginAsync(string email, string password, CancellationToken ct = default);

    /// <summary>
    /// Get current user session from HTTP context
    /// </summary>
    UserSession? GetCurrentUser();

    /// <summary>
    /// Check if user is authenticated
    /// </summary>
    bool IsAuthenticated();

    /// <summary>
    /// Save user session after successful login
    /// </summary>
    void SaveUserSession(LoginResponse response);

    /// <summary>
    /// Clear user session (logout)
    /// </summary>
    void ClearUserSession();

    /// <summary>
    /// Get JWT token from session
    /// </summary>
    string? GetToken();
}
