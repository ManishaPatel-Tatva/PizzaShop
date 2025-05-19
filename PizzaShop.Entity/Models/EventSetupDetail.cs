using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class EventSetupDetail
{
    public long Id { get; set; }

    public long SetupId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<EventSetupMapping> EventSetupMappings { get; set; } = new List<EventSetupMapping>();

    public virtual EventSetup Setup { get; set; } = null!;
}
