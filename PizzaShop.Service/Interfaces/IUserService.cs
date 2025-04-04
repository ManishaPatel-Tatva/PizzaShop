using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IUserService
{
    Task<User> GetUserByEmail(string email);
    
    Task<UsersListViewModel> GetPagedRecords(int pageSize, int pageNumber, string search);

    Task<AddUserViewModel> GetAddUser();

    Task<(bool success, string? message)> AddUserAsync(AddUserViewModel model, string createrEmail);

    Task<EditUserViewModel> GetUserAsync(long userId);

    Task<(bool success, string? message)> UpdateUser(EditUserViewModel model);

    Task<bool> DeleteUser(long id);

}
