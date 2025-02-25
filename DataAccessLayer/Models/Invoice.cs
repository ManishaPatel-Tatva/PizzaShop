using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class Invoice
{
    public long Id { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public string CustomerId { get; set; } = null!;

    public decimal CgstTax { get; set; }

    public decimal SgstTax { get; set; }

    public decimal GstTax { get; set; }

    public decimal OtherTax { get; set; }

    public decimal FinalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;
}
