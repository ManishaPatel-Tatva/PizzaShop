namespace PizzaShop.Entity.ViewModels;

public class AssignTableViewModel
{
    public List<WaitingTokenViewModel> WaitingList { get; set; } = new();
    public WaitingTokenViewModel CustomerDetail { get; set; } = new();
    public List<TableViewModel> Tables { get; set; } = new();
}
