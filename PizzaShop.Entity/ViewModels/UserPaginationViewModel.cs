namespace PizzaShop.Entity.ViewModels{
    public class UserPaginationViewModel
    {
        public IEnumerable<UserInfoViewModel>? Users { get; set; }
        public Pagination? Page { get; set; }
    }
}