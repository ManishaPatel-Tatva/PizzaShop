using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IProfileService
{
    public Task<ProfileViewModel> Get(string email);
    public Task<bool> Update(ProfileViewModel model);
    public Task<ResponseViewModel> ChangePassword(ChangePasswordViewModel model);
}
