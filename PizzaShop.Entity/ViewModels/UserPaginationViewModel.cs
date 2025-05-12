namespace PizzaShop.Entity.ViewModels
{
    public class UserPaginationViewModel
    {
        public IEnumerable<UserInfoViewModel> Users { get; set; } = new List<UserInfoViewModel>();
        public Pagination Page { get; set; } = new();
    }
}