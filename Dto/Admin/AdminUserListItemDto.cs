namespace AssetStore.Dto.Admin;

public class AdminUserListItemDto
{
    public string Id { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Roles { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }

    public bool CanPromoteToCreator { get; init; }
}
