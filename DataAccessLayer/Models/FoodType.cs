using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class FoodType
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();
}
