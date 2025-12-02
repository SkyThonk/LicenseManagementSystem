namespace LicenseManagement.Web.Constants;

/// <summary>
/// Centralized route constants for the application
/// </summary>
public static class AppRoutes
{
    public static class Dashboard
    {
        public const string Index = "/";
    }

    public static class Tenants
    {
        public const string Index = "/tenants";
        public const string Create = "/tenants/create";
        public const string Details = "/tenants/{id}";
        public const string Edit = "/tenants/edit/{id}";
    }

    public static class Licenses
    {
        public const string Index = "/licenses";
        public const string Create = "/licenses/create";
        public const string Details = "/licenses/{id}";
        public const string Edit = "/licenses/edit/{id}";
    }

    public static class Payments
    {
        public const string Index = "/payments";
        public const string Create = "/payments/create";
        public const string Details = "/payments/{id}";
    }

    public static class Documents
    {
        public const string Index = "/documents";
        public const string Upload = "/documents/upload";
        public const string Details = "/documents/{id}";
    }

    public static class Notifications
    {
        public const string Index = "/notifications";
    }

    public static class Account
    {
        public const string Login = "/account/login";
        public const string Logout = "/account/logout";
        public const string Profile = "/account/profile";
    }
}
