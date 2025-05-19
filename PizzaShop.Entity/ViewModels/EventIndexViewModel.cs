using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class EventIndexViewModel
{
    public long StatusId { get; set; } = 0;
    public string Status { get; set; } = "";
    public List<EventStatus> Statuses { get; set; } = new List<EventStatus>();
    public long TypeId { get; set; } = 0;
    public string Type { get; set; } = "";
    public List<EventType> Types { get; set; } = new List<EventType>();
}
