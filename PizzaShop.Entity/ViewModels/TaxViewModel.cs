using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels;

public class TaxViewModel
{
    public long TaxId { get; set; }
    [Required(ErrorMessage = "Tax Name is required.")]
    [StringLength(100, ErrorMessage = "Tax Name cannot exceed 100 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Type is required.")]
    public bool? IsPercentage { get; set; } = true;
    // public string Type { get; set; }

    [Display(Name = "Is Enabled")]
    public bool IsEnabled { get; set; } = true;

    [Display(Name = "Is Default")]
    public bool Default { get; set; } =true;

    // [Range(0, 100, ErrorMessage = "Tax Value must be between 0 and 100.")]
    [Required(ErrorMessage = "Tax Value is required.")]
    public decimal TaxValue { get; set; }
}