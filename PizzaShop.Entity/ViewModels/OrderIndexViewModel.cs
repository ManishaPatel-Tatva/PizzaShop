using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class OrderIndexViewModel
{
    public long StatusId { get; set; }
    public string Status { get; set; } = "";
    public List<OrderStatus> Statuses { get; set; } = new List<OrderStatus>();
}
