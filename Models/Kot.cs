using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class Kot
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public bool Ready { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Order Order { get; set; } = null!;
}
