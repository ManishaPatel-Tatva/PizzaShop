using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IUserService
{
    Task<AddUserViewModel> Get();
    Task<User> Get(string email);
    Task<EditUserViewModel> Get(long userId);
    Task<UserPaginationViewModel> Get(FilterViewModel filter);
    Task<ResponseViewModel> Add(AddUserViewModel model, string createrEmail);
    Task<ResponseViewModel> Update(EditUserViewModel model, string createrEmail);
    Task<bool> Delete(long id);

}
