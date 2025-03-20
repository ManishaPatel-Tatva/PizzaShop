using Microsoft.AspNetCore.Http;
using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModels;

public class TableViewModel
{
    public long TableId { get; set; }
    public string Name { get; set; }
    public long SectionId { get; set; } = 0;
    public int Capacity { get; set; }
    public long StatusId { get; set; } = 1;
    public string? StatusName { get; set; }
    public List<TableStatus>? StatusList { get; set; } = new List<TableStatus>();
    public List<Section>? SectionList { get; set; } = new List<Section>();
}
