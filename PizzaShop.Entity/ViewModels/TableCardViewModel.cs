namespace PizzaShop.Entity.ViewModels;

public class TableCardViewModel
{
    public long Id { get; set; } = 0;
    public string TableName { get; set; } = "";
    public string TableStatus { get; set; } = "";
    public decimal OrderAmount { get; set; } = 0;
    public int Capacity { get; set; }
    public DateTime OrderTime { get; set; }
    public long CustomerId { get; set; } = 0;
}
