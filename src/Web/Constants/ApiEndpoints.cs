namespace LicenseManagement.Web.Constants;

/// <summary>
/// API endpoint constants for backend services
/// </summary>
public static class ApiEndpoints
{
    public static class Tenant
    {
        public const string Base = "api/tenants";
        public const string GetAll = Base;
        public const string GetById = Base + "/{0}";
        public const string Create = Base;
        public const string Update = Base + "/{0}";
        public const string Delete = Base + "/{0}";
    }

    public static class License
    {
        public const string Base = "api/licenses";
        public const string GetAll = Base;
        public const string GetById = Base + "/{0}";
        public const string Create = Base;
        public const string Update = Base + "/{0}";
        public const string Delete = Base + "/{0}";
    }

    public static class Payment
    {
        public const string Base = "api/payments";
        public const string GetAll = Base;
        public const string GetById = Base + "/{0}";
        public const string Create = Base;
        public const string Process = Base + "/process/{0}";
        public const string Cancel = Base + "/cancel/{0}";
    }

    public static class Document
    {
        public const string Base = "api/documents";
        public const string GetAll = Base;
        public const string GetById = Base + "/{0}";
        public const string Upload = Base;
        public const string Delete = Base + "/{0}";
        public const string DownloadUrl = Base + "/{0}/download-url";
    }

    public static class Notification
    {
        public const string Base = "api/notifications";
        public const string GetAll = Base;
        public const string GetById = Base + "/{0}";
        public const string Send = Base;
    }

    public static class Auth
    {
        public const string Login = "api/auth/login";
        public const string Logout = "api/auth/logout";
        public const string Refresh = "api/auth/refresh";
    }
}
