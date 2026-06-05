namespace AssetStore.Models.Constants;

public static class AppRoles
{
    public const string Administrator = "Administrator";
    public const string Creator = "Creator";
    public const string User = "User";

    public static readonly string[] All = [Administrator, Creator, User];
}
