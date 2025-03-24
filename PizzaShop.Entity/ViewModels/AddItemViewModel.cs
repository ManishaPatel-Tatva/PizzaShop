using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class AddItemViewModel
{
    public long ItemId { get; set; } = 0;

    [Required(ErrorMessage = "Category is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Category is required")]
    public long CategoryId { get; set; }

    public List<Category> Categories {get; set;} = new List<Category>();
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Item Type is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Item Type is required")]
    public long ItemTypeId {get; set;}
    public List<FoodType> ItemTypes {get; set;} = new List<FoodType>();

    [Required(ErrorMessage = "Rate is required")]
    [Range(0.001, double.MaxValue, ErrorMessage = "Rate should be greater than 0")]
    public decimal Rate { get; set; } = decimal.Zero;

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity should be greater than 0")]
    public int Quantity {get; set;}

    [Required(ErrorMessage = "Unit is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Unit is required")]
    public long UnitId {get; set;}

    public List<Unit> Units  {get; set;} = new List<Unit>();

    [Required(ErrorMessage = "Available is required")]
    public bool Available{get; set;}

     [Required(ErrorMessage = "Default Tax is required")]
    public bool DefaultTax { get; set; }

    [Required(ErrorMessage = "Tax is required")]
    public decimal TaxPercentage { get; set; }

    public string? ShortCode { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; }

    public string? ItemImageUrl { get; set; } = "/images/dining-menu.png";

    public IFormFile? Image {get; set;}

    public long ModifierGroupId { get; set; }

    public List<ModifierGroup>? ModifierGroups { get; set; } = new List<ModifierGroup>();

    public List<ItemModifierViewModel>? ItemModifierGroups { get; set; } = new List<ItemModifierViewModel>();

}
