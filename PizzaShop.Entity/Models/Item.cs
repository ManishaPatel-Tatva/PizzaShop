using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Item
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public long CategoryId { get; set; }

    public long FoodTypeId { get; set; }

    public decimal Rate { get; set; }

    public decimal Tax { get; set; }

    public bool DefaultTax { get; set; }

    public int Quantity { get; set; }

    public long UnitId { get; set; }

    public bool Available { get; set; }

    public string? ShortCode { get; set; }

    public string ImageUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual FoodType FoodType { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Unit Unit { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
