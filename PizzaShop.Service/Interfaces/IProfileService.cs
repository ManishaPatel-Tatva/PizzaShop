using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IProfileService
{
    public Task<ProfileViewModel> Get();
    public Task Update(ProfileViewModel model);
    public Task<ResponseViewModel> ChangePassword(ChangePasswordViewModel model);
}
