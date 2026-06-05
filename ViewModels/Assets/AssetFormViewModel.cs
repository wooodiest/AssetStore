using System.ComponentModel.DataAnnotations;

namespace AssetStore.ViewModels.Assets;

public class AssetFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200)]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(4000)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, 999999.99)]
    [Display(Name = "Price (PLN, 0 = free)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Display(Name = "File")]
    public IFormFile? File { get; set; }

    [Display(Name = "Thumbnail")]
    public IFormFile? Thumbnail { get; set; }

    public string ThumbnailUrl { get; set; } = string.Empty;

    public IReadOnlyList<CategoryOptionViewModel> Categories { get; init; } = [];
}

public class CategoryOptionViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;
}

