using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IUserService
{
    // public List<Role> GetRoles();

    // public Task<UsersListViewModel> GetUsersListAsync(int pageNumber, int pageSize, string search);

    UsersListViewModel GetPagedRecords(int pageSize, int pageNumber);
    
    // Task<List<UserInfoViewModel>> GetUserInfoAsync();

    // Task<User?> GetUserByEmailAsync(string email);

    // EditUserViewModel GetUserByIdAsync(long id);

    // Task<AddUserViewModel> GetAddUser();

    // Task<bool> AddUserAsync(AddUserViewModel model, string createrEmail);

    // Task<bool> UpdateUser(EditUserViewModel model);

    // Task<bool> SoftDeleteUser(long id);
}
