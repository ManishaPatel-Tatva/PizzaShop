using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Table
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long SectionId { get; set; }

    public int Capacity { get; set; }

    public bool? IsAvailable { get; set; }

    public long StatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Section Section { get; set; } = null!;

    public virtual TableStatus Status { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<WaitingToken> WaitingTokens { get; set; } = new List<WaitingToken>();
}
