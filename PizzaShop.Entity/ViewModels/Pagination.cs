namespace PizzaShop.Entity.ViewModels;

public class Pagination
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalRecord { get; set; }
    public int FromRec { get; set; }
    public int ToRec { get; set; }
    public int PageSize { get; set; } = 5;
}
