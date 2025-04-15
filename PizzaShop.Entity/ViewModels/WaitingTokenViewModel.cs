using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class WaitingTokenViewModel
{
    public string? Email { get; set; }
    public string? Name { get; set; }
    public long Phone { get; set; }
    public int Members { get; set; }
    public long SectionId { get; set; }
    public List<SectionViewModel> Sections { get; set; } = new List<SectionViewModel>();
}
