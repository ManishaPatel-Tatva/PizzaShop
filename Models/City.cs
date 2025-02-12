using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class City
{
    public long Id { get; set; }

    public long? CountryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual City? Country { get; set; }

    public virtual ICollection<City> InverseCountry { get; set; } = new List<City>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
