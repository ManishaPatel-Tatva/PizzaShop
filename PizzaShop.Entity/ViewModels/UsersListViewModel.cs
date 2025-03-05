namespace PizzaShop.Entity.ViewModels{
    public class UsersListViewModel
    {
        public IEnumerable<UserInfoViewModel>? Users { get; set; }
        
        public Pagination? Page { get; set; }
    }
}