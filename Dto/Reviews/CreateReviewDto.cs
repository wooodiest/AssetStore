using System.ComponentModel.DataAnnotations;

namespace AssetStore.Dto.Reviews;

public class CreateReviewDto
{
    [Required]
    public int AssetId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Comment { get; set; } = string.Empty;
}
