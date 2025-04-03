using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Order
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public long StatusId { get; set; }

    public string? Instructions { get; set; }

    public decimal SubTotal { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public int Members { get; set; }

    public bool? IsDineIn { get; set; }

    public decimal FinalAmount { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<CustomersReview> CustomersReviews { get; set; } = new List<CustomersReview>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Kot> Kots { get; set; } = new List<Kot>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderTableMapping> OrderTableMappings { get; set; } = new List<OrderTableMapping>();

    public virtual ICollection<OrderTaxMapping> OrderTaxMappings { get; set; } = new List<OrderTaxMapping>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
