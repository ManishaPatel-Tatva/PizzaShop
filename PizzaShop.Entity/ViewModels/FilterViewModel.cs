namespace PizzaShop.Entity.ViewModels;

public class FilterViewModel
{
    public int PageSize { get; set; } = 5;
    public int PageNumber { get; set; } = 1;
    public string Status { get; set; } = "";
    public string DateRange { get; set; } = "";
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public string Column { get; set; } = "";
    public string Sort { get; set; } = "";
    public string Search { get; set; } = "";
}
