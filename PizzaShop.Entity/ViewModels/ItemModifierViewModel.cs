namespace PizzaShop.Entity.ViewModels;

public class ItemModifierViewModel
{
    public long ItemId { get; set;}
    public long ModifierGroupId { get; set;}
    public string ModifierGroupName { get; set; }
    public int MinAllowed { get; set; }
    public int MaxAllowed { get; set; }
    public List<ModifierViewModel> ModifierList { get; set; }
}
