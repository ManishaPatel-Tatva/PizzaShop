using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IUserService
{
    AddUserViewModel Get();
    Task<User> Get(string email);
    Task<EditUserViewModel> Get(long userId);
    Task<UserPaginationViewModel> Get(FilterViewModel filter);
    Task<ResponseViewModel> Add(AddUserViewModel user);
    Task<ResponseViewModel> Update(EditUserViewModel user);
    Task Delete(long id);
    Task<long> LoggedInUser();

}
