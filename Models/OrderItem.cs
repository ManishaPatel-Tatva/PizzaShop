using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class OrderItem
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long ItemId { get; set; }

    public int Quantity { get; set; }

    public string? Instructions { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<OrderItemsModifier> OrderItemsModifiers { get; set; } = new List<OrderItemsModifier>();

    public virtual User? UpdatedByNavigation { get; set; }
}
