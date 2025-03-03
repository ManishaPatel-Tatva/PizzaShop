namespace PizzaShop.Entity.ViewModels;

public class PermissionViewModel
{
    public long PermissionId { get; set; }
    public string PermissionName { get; set;}
    public bool CanView { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}
