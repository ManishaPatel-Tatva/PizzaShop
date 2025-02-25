using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Taxis
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public bool IsEnabled { get; set; }

    public bool DefaultTax { get; set; }

    public decimal TaxValue { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
