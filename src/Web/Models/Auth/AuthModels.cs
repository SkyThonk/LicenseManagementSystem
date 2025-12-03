namespace LicenseManagement.Web.Models.Auth;

/// <summary>
/// Login request model
/// </summary>
public class LoginViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Login response from API
/// </summary>
public class LoginResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// User session data stored in session
/// </summary>
public class UserSession
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
