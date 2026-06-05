using System.ComponentModel.DataAnnotations;

namespace AssetStore.Dto.Assets;

public class UpdateAssetDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, 999999.99)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    public int CategoryId { get; set; }

    public IFormFile? Thumbnail { get; set; }

    public string ExistingThumbnailUrl { get; set; } = string.Empty;
}

