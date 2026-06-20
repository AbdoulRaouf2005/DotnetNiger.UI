namespace DotnetNiger.UI.Helpers;

public static class RoleConstants
{
    public const string Admin = "admin";
    public const string SuperAdmin = "superadmin";
    public const string Moderator = "moderator";
    public const string Member = "Member";

    public static readonly string[] AdminRoles = [Admin, SuperAdmin, Moderator];

    public static bool IsAdminRole(string? role) =>
        !string.IsNullOrWhiteSpace(role) && AdminRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
}
