using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class ModifierMapping
{
    public long Id { get; set; }

    public long Modifierid { get; set; }

    public long Modifiergroupid { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Modifier Modifier { get; set; } = null!;

    public virtual ModifierGroup Modifiergroup { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
