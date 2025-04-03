using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Helpers;

public static class PaginationHelper
{
    public static void SetPagination(
        this Pagination page,
        int totalRecords,
        int pageSize,
        int pageNumber
    )
    {
        if (totalRecords <= 0)
            return;

        page.TotalRecord = totalRecords;
        page.FromRec = (pageNumber - 1) * pageSize;
        page.ToRec = page.FromRec + pageSize;

        if (page.ToRec > page.TotalRecord)
        {
            page.ToRec = page.TotalRecord;
        }

        page.FromRec += 1;
        page.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        page.CurrentPage = pageNumber;
        page.PageSize = pageSize;
    }
}
