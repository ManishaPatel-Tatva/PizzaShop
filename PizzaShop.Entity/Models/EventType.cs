using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class EventType
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
