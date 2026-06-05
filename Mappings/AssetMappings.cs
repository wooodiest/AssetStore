using AssetStore.Dto.Assets;
using AssetStore.Dto.Categories;
using AssetStore.Dto.Creator;
using AssetStore.Models;
using AssetStore.ViewModels.Assets;
using AssetStore.ViewModels.Creator;

namespace AssetStore.Mappings;

public static class AssetMappings
{
    public static AssetListItemDto ToListItemDto(Asset asset) => new()
    {
        Id = asset.Id,
        Title = asset.Title,
        Description = asset.Description,
        Price = asset.Price,
        CategoryName = asset.Category?.Name ?? string.Empty,
        CreatorName = asset.Creator?.Email ?? asset.Creator?.UserName ?? "Unknown",
        UploadDate = asset.UploadDate
        ,ThumbnailUrl = asset.ThumbnailUrl
    };

    public static ReviewItemDto ToReviewItemDto(Review review) => new()
    {
        Id = review.Id,
        UserName = review.User?.Email ?? review.User?.UserName ?? "Anonymous",
        Rating = review.Rating,
        Comment = review.Comment,
        PostedAt = review.PostedAt
    };

    public static CategoryOptionDto ToCategoryOptionDto(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name
    };

    public static CreatorAssetItemDto ToCreatorAssetItemDto(Asset asset) => new()
    {
        Id = asset.Id,
        Title = asset.Title,
        Price = asset.Price,
        CategoryName = asset.Category?.Name ?? string.Empty,
        UploadDate = asset.UploadDate
    };

    public static AssetCatalogViewModel ToViewModel(AssetCatalogResultDto dto) => new()
    {
        Items = dto.Assets.Items.Select(i => new AssetListItemViewModel
        {
            Id = i.Id,
            Title = i.Title,
            Description = i.Description,
            Price = i.Price,
            CategoryName = i.CategoryName,
            CreatorName = i.CreatorName,
            UploadDate = i.UploadDate,
            ThumbnailUrl = i.ThumbnailUrl
        }).ToList(),
        Page = dto.Assets.Page,
        PageSize = dto.Assets.PageSize,
        TotalCount = dto.Assets.TotalCount,
        TotalPages = dto.Assets.TotalPages,
        HasPreviousPage = dto.Assets.HasPreviousPage,
        HasNextPage = dto.Assets.HasNextPage,
        Categories = dto.Categories.Select(c => new CategoryOptionViewModel
        {
            Id = c.Id,
            Name = c.Name
        }).ToList(),
        SelectedCategoryId = dto.SelectedCategoryId,
        SelectedMaxPrice = dto.SelectedMaxPrice
    };

    public static AssetDetailsViewModel ToViewModel(AssetDetailsDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title,
        Description = dto.Description,
        Price = dto.Price,
        CategoryName = dto.CategoryName,
        CreatorName = dto.CreatorName,
        UploadDate = dto.UploadDate,
        AverageRating = dto.AverageRating,
        Reviews = dto.Reviews.Select(r => new ReviewItemViewModel
        {
            Id = r.Id,
            UserName = r.UserName,
            Rating = r.Rating,
            Comment = r.Comment,
            PostedAt = r.PostedAt
        }).ToList(),
        ShowLoginPrompt = dto.ShowLoginPrompt,
        CanPurchase = dto.CanPurchase,
        CanDownload = dto.CanDownload,
        HasPurchased = dto.HasPurchased,
        IsOwner = dto.IsOwner,
        CanReview = dto.CanReview,
        HasReviewed = dto.HasReviewed
        ,ThumbnailUrl = dto.ThumbnailUrl
    };

    public static AssetFormViewModel ToCreateViewModel(IReadOnlyList<CategoryOptionDto> categories) => new()
    {
        Categories = categories.Select(c => new CategoryOptionViewModel { Id = c.Id, Name = c.Name }).ToList()
    };

    public static AssetFormViewModel ToEditViewModel(UpdateAssetDto dto, int id, IReadOnlyList<CategoryOptionDto> categories) => new()
    {
        Id = id,
        Title = dto.Title,
        Description = dto.Description,
        Price = dto.Price,
        CategoryId = dto.CategoryId,
        ThumbnailUrl = dto.ExistingThumbnailUrl,
        Categories = categories.Select(c => new CategoryOptionViewModel { Id = c.Id, Name = c.Name }).ToList()
    };

    public static CreatorDashboardViewModel ToViewModel(CreatorDashboardDto dto) => new()
    {
        AssetCount = dto.AssetCount,
        TransactionCount = dto.TransactionCount,
        Assets = dto.Assets.Select(a => new CreatorAssetItemViewModel
        {
            Id = a.Id,
            Title = a.Title,
            Price = a.Price,
            CategoryName = a.CategoryName,
            UploadDate = a.UploadDate
        }).ToList()
    };
}

