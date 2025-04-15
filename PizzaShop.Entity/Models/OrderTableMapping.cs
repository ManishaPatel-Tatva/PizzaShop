using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class OrderTableMapping
{
    public long Id { get; set; }

    public long? OrderId { get; set; }

    public long TableId { get; set; }

    public long CustomerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Table Table { get; set; } = null!;
}
