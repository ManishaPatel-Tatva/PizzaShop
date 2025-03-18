using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class ModifierGroup
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ItemModifierGroup> ItemModifierGroups { get; set; } = new List<ItemModifierGroup>();

    public virtual ICollection<ModifierMapping> ModifierMappings { get; set; } = new List<ModifierMapping>();
}
