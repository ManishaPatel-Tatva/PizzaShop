namespace PizzaShop.Entity.ViewModels;

public class ResponseViewModel
{
    public long EntityId { get; set; } = 0;
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public string ExMessage { get; set; } = "";
}
