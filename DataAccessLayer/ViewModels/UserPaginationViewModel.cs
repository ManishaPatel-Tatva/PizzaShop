using DataAccessLayer.Models;

namespace DataAccessLayer.ViewModels;

public class UserPaginationViewModel
{
    public IEnumerable<UserInfoViewModel>? Users { get; set; }
    public PaginationViewModel Page { get; set; }
}
