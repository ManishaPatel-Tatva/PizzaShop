using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class FavouriteItem
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public long ItemId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Customer Item { get; set; } = null!;
}
