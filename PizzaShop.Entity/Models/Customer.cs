using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Customer
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Phone { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<CustomersReview> CustomersReviews { get; set; } = new List<CustomersReview>();

    public virtual ICollection<FavouriteItem> FavouriteItemCustomers { get; set; } = new List<FavouriteItem>();

    public virtual ICollection<FavouriteItem> FavouriteItemItems { get; set; } = new List<FavouriteItem>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<WaitingToken> WaitingTokens { get; set; } = new List<WaitingToken>();
}
