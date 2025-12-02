namespace LicenseManagement.Web.Constants;

/// <summary>
/// UI-related constants
/// </summary>
public static class UiConstants
{
    public const string AppName = "License Management";
    public const string AppShortName = "LMS";
    public const int DefaultPageSize = 10;
    public static readonly int[] PageSizeOptions = [10, 25, 50, 100];

    public static class Colors
    {
        public const string Primary = "#6366f1";
        public const string Secondary = "#64748b";
        public const string Success = "#22c55e";
        public const string Warning = "#f59e0b";
        public const string Danger = "#ef4444";
        public const string Info = "#3b82f6";
    }

    public static class Icons
    {
        public const string Dashboard = "bi-grid-1x2";
        public const string Tenants = "bi-building";
        public const string Licenses = "bi-file-earmark-text";
        public const string Payments = "bi-credit-card";
        public const string Documents = "bi-folder";
        public const string Notifications = "bi-bell";
        public const string Settings = "bi-gear";
        public const string Profile = "bi-person-circle";
        public const string Logout = "bi-box-arrow-right";
        public const string Search = "bi-search";
        public const string Add = "bi-plus-lg";
        public const string Edit = "bi-pencil";
        public const string Delete = "bi-trash";
        public const string View = "bi-eye";
        public const string Download = "bi-download";
        public const string Upload = "bi-upload";
    }
}
