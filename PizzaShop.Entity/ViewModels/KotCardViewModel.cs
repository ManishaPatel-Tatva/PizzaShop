namespace PizzaShop.Entity.ViewModels;

public class KotCardViewModel
{
    public long OrderId { get; set; }
    public string SectionName { get; set; } = "";
    public List<string> Tables { get; set; } = new List<string>();
    public DateTime Time { get; set; }
    public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    public string Instruction { get; set; } = "";
}
