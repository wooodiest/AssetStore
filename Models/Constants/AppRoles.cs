namespace AssetStore.Models.Constants;

public static class AppRoles
{
    public const string Administrator = "Administrator";
    public const string Creator = "Creator";
    public const string User = "User";

    public const string CreatorOrAdministrator = "Creator,Administrator";

    public static readonly string[] All = [Administrator, Creator, User];
}
