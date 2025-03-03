using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Modifier
{
    public long Id { get; set; }

    public long ModifierGroupId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public long FoodTypeId { get; set; }

    public decimal Rate { get; set; }

    public int Quantity { get; set; }

    public long UnitId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual FoodType FoodType { get; set; } = null!;

    public virtual ModifierGroup ModifierGroup { get; set; } = null!;

    public virtual Unit Unit { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
