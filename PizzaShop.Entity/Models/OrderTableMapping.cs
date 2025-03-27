using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class OrderTableMapping
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long TableId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;
}
