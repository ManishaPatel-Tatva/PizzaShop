namespace PizzaShop.Entity.ViewModels;

public class RolePermissionViewModel
{
    public List<PermissionViewModel> Permissions{ get; set; }
    public long RoleId { get; set; }
    public string RoleName { get; set; }
}
