using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Order
{
    public long Id { get; set; }

    public DateOnly Date { get; set; }

    public long CustomerId { get; set; }

    public long StatusId { get; set; }

    public long PaymentMethodId { get; set; }

    public int Rating { get; set; }

    public string? Instructions { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<CustomersReview> CustomersReviews { get; set; } = new List<CustomersReview>();

    public virtual ICollection<Kot> Kots { get; set; } = new List<Kot>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
