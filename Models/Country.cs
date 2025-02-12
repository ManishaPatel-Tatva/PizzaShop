using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class Country
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<State> States { get; set; } = new List<State>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
