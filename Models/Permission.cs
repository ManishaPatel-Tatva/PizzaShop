using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class Permission
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
