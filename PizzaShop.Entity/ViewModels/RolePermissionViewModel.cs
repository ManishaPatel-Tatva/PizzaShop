namespace PizzaShop.Entity.ViewModels;

public class RolePermissionViewModel
{
    public List<PermissionViewModel> Permissions{ get; set; } = new();
    public long RoleId { get; set; }
    public string RoleName { get; set; } = "";
}
