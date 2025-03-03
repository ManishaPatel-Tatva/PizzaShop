using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Unit
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();
}
