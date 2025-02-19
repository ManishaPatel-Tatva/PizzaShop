using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class City
{
    public long Id { get; set; }

    public long? StateId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<City> InverseState { get; set; } = new List<City>();

    public virtual City? State { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
