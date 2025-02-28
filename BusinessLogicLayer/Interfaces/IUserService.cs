using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

public interface IUserService
{
    public List<Role> GetRoles();

    public Task<UsersListViewModel> GetUsersListAsync(int pageNumber, int pageSize, string search);

    // public Task<List<UserInfoViewModel>> GetUserInfoAsync();

    public  Task<User?> GetUserByEmailAsync(string email);

    public EditUserViewModel GetUserByIdAsync(long id);

    public Task<AddUserViewModel> GetAddUser();

    public Task<bool> AddUserAsync(AddUserViewModel model, string createrEmail);

    public Task<bool> UpdateUser(EditUserViewModel model);

    public Task<bool> SoftDeleteUser(long id);
}
