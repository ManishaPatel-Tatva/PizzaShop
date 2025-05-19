using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class EventSetupMapping
{
    public long Id { get; set; }

    public long EventId { get; set; }

    public long SetupDetailId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual EventSetupDetail SetupDetail { get; set; } = null!;
}
