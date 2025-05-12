namespace PizzaShop.Entity.ViewModels;

public class DashboardViewModel
{
    public int TotalOrders { get; set; } = 0;
    public decimal TotalSales { get; set; } = 0;
    public decimal AvgOrderValue { get; set; } = 0;
    public int WaitingListCount { get; set; } = 0;
    public double AvgWaitingTime { get; set; } = 0;
    public int NewCustomerCount { get; set; } = 0;
    public List<SellingItem> TopSellingItems { get; set; } = new();
    public List<SellingItem> LeastSellingItems { get; set; } = new();
    public List<decimal> Revenue { get; set; } = new();
    public List<int> CustomerCount { get; set; } = new();
}
 
public class SellingItem
{
    public string Name { get; set; } = null!;
    public string? ImgUrl { get; set; } 
    public int TotalQuantity { get; set; } = 0;
}
 
public class RevenuData
{
    public int Date { get; set; }
    public decimal Revenue { get; set; } = 0;
}
 
public class CustomerData{
    public int Month { get; set; }
    public int CustomerCount { get; set; } = 0;
}
 
