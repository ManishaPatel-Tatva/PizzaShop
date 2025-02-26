namespace DataAccessLayer.ViewModels{
    public class UsersListViewModel
    {
        public List<UserInfoViewModel> User { get; set; } = new List<UserInfoViewModel>();
        
        public int TotalRecords { get; set; }
    }
}