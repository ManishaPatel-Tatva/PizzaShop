using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class EventCancellation
{
    public long Id { get; set; }

    public long EventId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
