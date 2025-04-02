using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class OrderTaxMapping
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long TaxId { get; set; }

    public decimal TaxValue { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Taxis Tax { get; set; } = null!;
}
