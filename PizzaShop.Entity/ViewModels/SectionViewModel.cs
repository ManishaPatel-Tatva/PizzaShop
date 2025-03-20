namespace PizzaShop.Entity.ViewModels;

public class SectionViewModel
{
    public long SectionId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; } = null!;
}
