namespace LicenseManagement.Web.Constants;

/// <summary>
/// API endpoint constants for backend services
/// </summary>
public static class ApiEndpoints
{
    public static class Tenant
    {
        public const string Base = "api/tenant";
        public const string Register = Base + "/register";
        public const string List = Base + "/list";
        public const string Profile = Base + "/profile";
        public const string Update = Base + "/profile/{0}";
        public const string Activate = Base + "/{0}/activate";
        public const string Deactivate = Base + "/{0}/deactivate";
    }

    public static class License
    {
        public const string Base = "api/Licenses";
        public const string GetAll = Base;
        public const string GetById = Base + "/{0}";
        public const string Create = Base;
        public const string Update = Base + "/{0}";
        public const string Delete = Base + "/{0}";
    }

    public static class LicenseType
    {
        public const string Base = "api/LicenseTypes";
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
        public const string Download = Base + "/{0}/download";
        public const string DownloadUrl = Base + "/{0}/download-url";
    }

    public static class Notification
    {
        public const string Base = "api/notifications";
        public const string GetAll = Base;
        public const string GetById = Base + "/{0}";
        public const string Send = Base;
        public const string UnreadCount = Base + "/unread-count";
        public const string MarkAsRead = Base + "/{0}/mark-as-read";
        public const string MarkAllAsRead = Base + "/mark-all-as-read";
        public const string Delete = Base + "/{0}";
    }

    public static class Auth
    {
        public const string Login = "api/authentication/login";
        public const string Logout = "api/authentication/logout";
        public const string Refresh = "api/authentication/refresh";
    }
}
