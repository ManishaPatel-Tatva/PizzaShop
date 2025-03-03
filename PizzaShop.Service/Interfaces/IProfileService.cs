using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IProfileService
{
    public Task<MyProfileViewModel> GetMyProfileAsync(string email);
    public Task<bool> UpdateProfileAsync(MyProfileViewModel model);
    public Task<bool> ChangePasswordAsync(ChangePasswordViewModel model);
}
