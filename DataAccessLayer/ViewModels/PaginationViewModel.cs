namespace DataAccessLayer.ViewModels;

public class PaginationViewModel
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int TotalRecord { get; set; }
    public int FromRec { get; set; }
    public int ToRec { get; set; }
}
