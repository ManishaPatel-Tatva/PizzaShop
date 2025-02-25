using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class CustomersReview
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public long OrderId { get; set; }

    public int FoodRating { get; set; }

    public int AmbienceRating { get; set; }

    public int EnvRating { get; set; }

    public int? Rating { get; set; }

    public string Review { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
