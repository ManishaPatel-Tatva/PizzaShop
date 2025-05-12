using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using PizzaShop.Service.Common;
using PizzaShop.Service.Exceptions;


namespace PizzaShop.Service.Services;
public class ProfileService : IProfileService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IAddressService _addressService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;

    public ProfileService(IGenericRepository<User> userRepository, IAddressService addressService, IGenericRepository<Role> roleRepository, IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _userRepository = userRepository;
        _addressService = addressService;
        _roleRepository = roleRepository;
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    /*-----------------------------------------------------------------My Profile---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    #region My Profile


    public async Task<ProfileViewModel> Get()
    {
        long userId = await _userService.LoggedInUser();

        User user = _userRepository.GetByCondition(
            predicate: u => u.Id == userId && !u.IsDeleted,
            includes: new List<Expression<Func<User, object>>>
            {
                u => u.Role
            }
            ).Result.FirstOrDefault()
            ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "User"));

        ProfileViewModel userProfile = new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.Name,
            UserName = user.Username,
            Email = user.Email,
            Phone = user.Phone,
            CountryId = user.CountryId,
            StateId = user.StateId,
            CityId = user.CityId,
            Address = user.Address,
            ZipCode = user.ZipCode,
            ProfileImageUrl = user.ProfileImg,
            Countries = _addressService.GetCountries(),
            States = _addressService.GetStates(user.CountryId),
            Cities = _addressService.GetCities(user.StateId)
        };

        return userProfile;
    }

    #endregion

    /*---------------------------------------------------------------Update Profile---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    #region Update Profile

    public async Task Update(ProfileViewModel model)
    {
        User user = await _userRepository.GetByStringAsync(u => u.Email == model.Email)
                    ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "User"));

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Username = model.UserName;
        user.Phone = model.Phone;
        user.CountryId = model.CountryId;
        user.StateId = model.StateId;
        user.CityId = model.CityId;
        user.Address = model.Address;
        user.ZipCode = model.ZipCode;

        // Handle Image Upload
        if (model.Image != null)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{Guid.NewGuid()}_{model.Image.FileName}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(stream);
            }

            user.ProfileImg = $"/uploads/{fileName}";
        }

        _httpContextAccessor.HttpContext.Response.Cookies.Delete("userName");
        _httpContextAccessor.HttpContext.Response.Cookies.Delete("profileImg");

        CookieOptions options = new()
        {
            Expires = DateTime.Now.AddDays(1),
            HttpOnly = true,
            IsEssential = true
        };

        _httpContextAccessor.HttpContext.Response.Cookies.Append("userName", user.Username, options);
        _httpContextAccessor.HttpContext.Response.Cookies.Append("profileImg", user.ProfileImg, options);

        await _userRepository.UpdateAsync(user);
    }

    #endregion

    /*---------------------------------------------------------------Change Password---------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    #region Change Password

    public async Task<ResponseViewModel> ChangePassword(ChangePasswordViewModel model)
    {
        User user = await _userRepository.GetByStringAsync(u => u.Email == model.Email)
        ?? throw new NotFoundException(NotificationMessages.NotFound.Replace("{0}", "User"));

        ResponseViewModel response = new();

        //Verify Password
        if (!PasswordHelper.VerifyPassword(model.OldPassword, user.Password))
        {
            response.Success = false;
            response.Message = NotificationMessages.Invalid.Replace("{0}", "Old Password");
            return response;
        }

        //Hash and Update Password
        user.Password = PasswordHelper.HashPassword(model.NewPassword);
        await _userRepository.UpdateAsync(user);

        response.Success = true;
        response.Message = NotificationMessages.Updated.Replace("{0}", "Password");
        return response;
    }

    #endregion

}

