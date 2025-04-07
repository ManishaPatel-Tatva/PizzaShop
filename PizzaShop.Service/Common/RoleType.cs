using System.ComponentModel;

namespace PizzaShop.Service.Common;

public enum RoleType
{
    Admin,
    [Description("Account Manager")]
    Account_Manager,
    Chef
}
