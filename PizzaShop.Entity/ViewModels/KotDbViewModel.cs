namespace PizzaShop.Entity.ViewModels;

public class KotDbViewModel
{
    public long OrderId { get; set; }
    public string SectionName { get; set; } = "";
    public string[] Tables { get; set; }        //array of string
    public DateTime Time { get; set; }
    public string Items { get; set; } = "[]";     //Json
    public string? Instruction { get; set; } = "";
}
