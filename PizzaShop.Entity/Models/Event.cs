using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Event
{
    public long Id { get; set; }

    public long? OrderId { get; set; }

    public long CustomerId { get; set; }

    public long EventTypeId { get; set; }

    public DateOnly Date { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int Members { get; set; }

    public bool IsDineIn { get; set; }

    public bool IsAcHall { get; set; }

    public string? SpecialInstruction { get; set; }

    public string Address { get; set; } = null!;

    public long SetupId { get; set; }

    public long StatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<EventCancellation> EventCancellations { get; set; } = new List<EventCancellation>();

    public virtual ICollection<EventSetupMapping> EventSetupMappings { get; set; } = new List<EventSetupMapping>();

    public virtual EventType EventType { get; set; } = null!;

    public virtual Order? Order { get; set; }

    public virtual EventSetup Setup { get; set; } = null!;

    public virtual EventStatus Status { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
