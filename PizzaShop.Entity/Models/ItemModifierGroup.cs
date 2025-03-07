using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class ItemModifierGroup
{
    public long Id { get; set; }

    public long ItemId { get; set; }

    public long ModifierGroupId { get; set; }

    public int MinAllowed { get; set; }

    public int MaxAllowed { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;

    public virtual ModifierGroup ModifierGroup { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
