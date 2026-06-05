using System.ComponentModel.DataAnnotations;

namespace AssetStore.ViewModels.Admin;

public class CategoryFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Nazwa jest wymagana.")]
    [StringLength(100)]
    [Display(Name = "Nazwa")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Opis jest wymagany.")]
    [StringLength(500)]
    [Display(Name = "Opis")]
    public string Description { get; set; } = string.Empty;
}
