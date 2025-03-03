using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class WaitingToken
{
    public long Id { get; set; }

    public long? TableId { get; set; }

    public long CustomerId { get; set; }

    public long SectionId { get; set; }

    public int Members { get; set; }

    public bool IsAssigned { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Section Section { get; set; } = null!;

    public virtual Table? Table { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
