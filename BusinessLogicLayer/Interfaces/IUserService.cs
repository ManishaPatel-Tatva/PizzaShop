using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

public interface IUserService
{
    public Task<UsersListViewModel> GetUsersListAsync();
    public Task<List<UserInfoViewModel>> GetUserInfoAsync();

    public  Task<User?> GetUserByEmailAsync(string email);

    // public Task<User?> GetUserByIdAsync(long id);

    public Task<AddUserViewModel> GetAddUser();
    public  Task AddUserAsync(AddUserViewModel model, string token);
    
    public Task UpdateUserAsync(User user);
}
