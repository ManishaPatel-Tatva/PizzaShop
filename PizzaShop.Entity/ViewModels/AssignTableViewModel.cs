namespace PizzaShop.Entity.ViewModels;

public class AssignTableViewModel
{
    public List<WaitingTokenViewModel> WaitingList { get; set; } = new List<WaitingTokenViewModel>();
    public WaitingTokenViewModel CustomerDetail { get; set; } = new();
}
