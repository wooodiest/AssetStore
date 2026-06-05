using AssetStore.Dto.Admin;
using AssetStore.Dto.Categories;
using AssetStore.ViewModels.Admin;

namespace AssetStore.Mappings;

public static class AdminMappings
{
    public static AdminDashboardViewModel ToViewModel(AdminDashboardDto dto) => new()
    {
        UserCount = dto.UserCount,
        CategoryCount = dto.CategoryCount,
        AssetCount = dto.AssetCount,
        ReviewCount = dto.ReviewCount
    };

    public static CategoryListViewModel ToViewModel(IReadOnlyList<CategoryDto> categories) => new()
    {
        Categories = categories.Select(c => new CategoryItemViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList()
    };

    public static CategoryFormViewModel ToFormViewModel(CategoryDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description
    };

    public static AdminUserListViewModel ToViewModel(IReadOnlyList<AdminUserListItemDto> users) => new()
    {
        Users = users.Select(u => new AdminUserItemViewModel
        {
            Id = u.Id,
            Email = u.Email,
            Roles = u.Roles,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            CanPromoteToCreator = u.CanPromoteToCreator
        }).ToList()
    };

    public static AdminReviewListViewModel ToViewModel(IReadOnlyList<AdminReviewListItemDto> reviews) => new()
    {
        Reviews = reviews.Select(r => new AdminReviewItemViewModel
        {
            Id = r.Id,
            AssetId = r.AssetId,
            AssetTitle = r.AssetTitle,
            UserName = r.UserName,
            Rating = r.Rating,
            Comment = r.Comment,
            PostedAt = r.PostedAt
        }).ToList()
    };

}
