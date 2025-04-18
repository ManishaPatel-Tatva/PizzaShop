﻿using System;
using System.Collections.Generic;

namespace PizzaShop.Entity.Models;

public partial class Invoice
{
    public long Id { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public long OrderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
