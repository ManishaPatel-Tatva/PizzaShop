namespace PizzaShop.Entity.ViewModels;

public class CustomerViewModel
{
    public long CustomerId { get; set; } = 0;
    public string Name { get; set; }
    public string Email { get; set; }
    public long Phone { get; set; }
    public DateOnly Date { get; set; }
    public int TotalOrder { get; set; } = 0;
}
