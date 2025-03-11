using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IUserService
{
    // public List<Role> GetRoles();

    // public Task<UsersListViewModel> GetUsersListAsync(int pageNumber, int pageSize, string search);
    
    // Task<List<UserInfoViewModel>> GetUserInfoAsync();

    // Task<User?> GetUserByEmailAsync(string email);


    // UsersListViewModel GetPagedRecords(int pageSize, int pageNumber);

    Task<UsersListViewModel> GetPagedRecords(int pageSize, int pageNumber, string search);

    Task<AddUserViewModel> GetAddUser();

    Task<(bool success, string? message)> AddUserAsync(AddUserViewModel model, string createrEmail);

    Task<EditUserViewModel> GetUserAsync(long userId);

    Task<(bool success, string? message)> UpdateUser(EditUserViewModel model);

    Task<bool> SoftDeleteUser(long id);

}
