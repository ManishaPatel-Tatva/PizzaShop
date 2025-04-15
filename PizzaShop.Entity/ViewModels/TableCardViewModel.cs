namespace PizzaShop.Entity.ViewModels;

public class TableCardViewModel
{
    public long Id { get; set; }
    public string? TableName { get; set; }
    public string? TableStatus { get; set; }
    public decimal? OrderAmount { get; set; }
    public int Capacity { get; set; }
    public DateTime OrderTime { get; set; }
}
