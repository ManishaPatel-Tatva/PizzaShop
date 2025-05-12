namespace PizzaShop.Entity.ViewModels;

public class KotViewModel
{
    public long CategoryId { get; set; } = 0;
    public string CategoryName { get; set; } = "";
    public bool IsReady { get; set; }
    public List<KotCardViewModel> KotCards { get; set; } = new List<KotCardViewModel>();
    public Pagination Page { get; set; }= new();
}
