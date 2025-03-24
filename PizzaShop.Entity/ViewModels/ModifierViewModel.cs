using System.ComponentModel.DataAnnotations;
using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class ModifierViewModel
{
    public long ModifierId { get; set;} = 0;

    public long ModifierGroupId { get; set; } = 0;
    public List<ModifierGroup>? ModifierGroups { get; set; } = new List<ModifierGroup>();
    public List<long>  SelectedMgList{ get; set; } = new List<long>();


    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    public string ModifierName { get; set;}


    [Required(ErrorMessage = "Unit is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Unit is required")]
    public long UnitId { get; set; }
    public string? UnitName { get; set; } = null!;
    public List<Unit>? Units { get; set; } = new List<Unit>();


    [Required(ErrorMessage = "Rate is required")]
    [Range(0.001, double.MaxValue, ErrorMessage = "Rate should be greater than 0")]
    public decimal Rate { get; set; }


    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity should be greater than 0")]
    public int Quantity { get; set; }


    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = null!;
}
