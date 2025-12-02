namespace Common.Domain.Constants;

public static class PermissionConstants
{
    // User Management Permissions
    public const string ReadUsers = "ReadUsers";
    public const string CreateUsers = "CreateUsers";
    public const string UpdateUsers = "UpdateUsers";
    public const string DeleteUsers = "DeleteUsers";

    // Role Management Permissions
    public const string ReadRoles = "ReadRoles";
    public const string CreateRoles = "CreateRoles";
    public const string UpdateRoles = "UpdateRoles";
    public const string DeleteRoles = "DeleteRoles";
    public const string AssignRoles = "AssignRoles";

    // Permission Management Permissions
    public const string ReadPermissions = "ReadPermissions";
    public const string CreatePermissions = "CreatePermissions";
    public const string UpdatePermissions = "UpdatePermissions";
    public const string DeletePermissions = "DeletePermissions";

    // Tenant Management Permissions
    public const string ReadTenants = "ReadTenants";
    public const string CreateTenants = "CreateTenants";
    public const string UpdateTenants = "UpdateTenants";
    public const string DeleteTenants = "DeleteTenants";
    public const string ManageTenantSettings = "ManageTenantSettings";


    // Permission Groups
    public static class Groups
    {
        public const string UserManagement = "User Management";
        public const string RoleManagement = "Role Management";
        public const string PermissionManagement = "Permission Management";
        public const string TenantManagement = "Tenant Management";
    }

    // Helper Methods
    public static bool IsValidPermission(string permission)
    {
        return AllPermissions.Contains(permission);
    }

    public static IEnumerable<string> GetAllPermissions()
    {
        return AllPermissions;
    }

    public static IEnumerable<string> GetPermissionsByGroup(string groupName)
    {
        return groupName switch
        {
            Groups.UserManagement => new[] { ReadUsers, CreateUsers, UpdateUsers, DeleteUsers },
            Groups.RoleManagement => new[] { ReadRoles, CreateRoles, UpdateRoles, DeleteRoles, AssignRoles },
            Groups.PermissionManagement => new[] { ReadPermissions, CreatePermissions, UpdatePermissions, DeletePermissions },
            Groups.TenantManagement => new[] { ReadTenants, CreateTenants, UpdateTenants, DeleteTenants, ManageTenantSettings },
            _ => Array.Empty<string>()
        };
    }

    private static readonly HashSet<string> AllPermissions = new()
    {
        // User Management
        ReadUsers, CreateUsers, UpdateUsers, DeleteUsers,

        // Role Management
        ReadRoles, CreateRoles, UpdateRoles, DeleteRoles, AssignRoles,

        // Permission Management
        ReadPermissions, CreatePermissions, UpdatePermissions, DeletePermissions,

        // Tenant Management
        ReadTenants, CreateTenants, UpdateTenants, DeleteTenants, ManageTenantSettings,

    };
}
