using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class AddItemViewModel
{
    public long ItemId { get; set; }

    public long CategoryId { get; set; }

    public List<Category> Categories {get; set;} = new List<Category>();
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    public required string Name { get; set; }

    public long ItemTypeId {get; set;}
    public List<FoodType> ItemTypes {get; set;} = new List<FoodType>();

    [Required(ErrorMessage = "Rate is required")]
    public decimal Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    public int Quantity {get; set;}

    public long UnitId {get; set;}

    public List<Unit> Units  {get; set;} = new List<Unit>();

    public bool Available{get; set;}

    public bool DefaultTax { get; set; }

    public decimal TaxPercentage { get; set; }

    public string? ShortCode { get; set; }

    public string? Description { get; set; }

    public string? ItemImageUrl { get; set; }

    public IFormFile? Image {get; set;}

    public long ModifierGroupId { get; set; }
    public List<ModifierGroup> ModifierGroups = new List<ModifierGroup>();


}
