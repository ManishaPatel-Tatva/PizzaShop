using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class OrderItemsModifier
{
    public long Id { get; set; }

    public long OrderItemId { get; set; }

    public long ModifierId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Modifier Modifier { get; set; } = null!;

    public virtual OrderItem OrderItem { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
