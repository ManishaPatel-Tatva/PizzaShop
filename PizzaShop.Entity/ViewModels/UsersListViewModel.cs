namespace PizzaShop.Entity.ViewModels{
    public class UsersListViewModel
    {
        public IEnumerable<UserInfoViewModel> User { get; set; } = new List<UserInfoViewModel>();
        
        public Pagination Page { get; set; }
    }
}