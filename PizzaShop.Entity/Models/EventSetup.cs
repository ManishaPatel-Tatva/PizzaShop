using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class EventSetup
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Rate { get; set; }

    public virtual ICollection<EventSetupDetail> EventSetupDetails { get; set; } = new List<EventSetupDetail>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
