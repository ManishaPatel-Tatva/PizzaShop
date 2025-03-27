using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Payment
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long PaymentMethodId { get; set; }

    public DateTime Date { get; set; }

    public bool IsPaid { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
