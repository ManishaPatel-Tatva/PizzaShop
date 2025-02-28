using BusinessLogicLayer.Helpers;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using BusinessLogicLayer.Interfaces;

namespace BusinessLogicLayer.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly PasswordHelper _passwordHelper;
    private readonly IEmailService _emailService;

    public UserService(IUserRepository userRepository, ICountryRepository countryRepository, PasswordHelper passwordHelper, IEmailService emailService)
    {
        _userRepository = userRepository;
        _countryRepository = countryRepository;
        _passwordHelper = passwordHelper;
        _emailService = emailService;
    }

/*----------------------------------------------------------------Display User List--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Display User List

    public async Task<UsersListViewModel> GetUsersListAsync(int pageNumber, int pageSize, string search)
    {
        var userList = await _userRepository.GetUsersInfoAsync(pageNumber, pageSize, search);
        return userList;
    }
#endregion

/*----------------------------------------------------------------Add User--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region  Add User

    //This method is used for getting the countries in 
    public async Task<AddUserViewModel> GetAddUser()
    {
        AddUserViewModel newUser = new AddUserViewModel
        {
            Countries = _countryRepository.GetCountries(),
            Roles = _userRepository.GetRoles()
        };

        return newUser;
    }

    public async Task<bool> AddUserAsync(AddUserViewModel model, string createrEmail)
    {
        var password = model.Password;
        model.Password = _passwordHelper.EncryptPassword(password);

        var success = await _userRepository.AddUserAsync(model, createrEmail);
       
       if(success)
       {
            string body = $@"
                <div style='background-color: #F2F2F2;'>
                    <div style='background-color: #0066A8; color: white; height: 90px; font-size: 40px; font-weight: 600; text-align: center; padding-top: 40px; margin-bottom: 0px;'>PIZZASHOP</div>
                    <div style='font-family:Verdana, Geneva, Tahoma, sans-serif; margin-top: 0px; font-size: 20px; padding: 10px;'>
                        <p>Pizza shop,</p>
                        <h3>Your Password is : {password}</h3>
                        <p>If you encounter any issues or have any question, please do not hesitate to contact our support team.</p>
                    </div>
                </div>";

            await _emailService.SendEmailAsync(model.Email, "New User", body);
       }

        return true;
    }
#endregion

/*----------------------------------------------------------------Edit User--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Edit User

    public EditUserViewModel GetUserByIdAsync(long id)
    {
        var user = _userRepository.GetUserByIdAsync(id);
        if (user == null)
            return null;

        user.Roles =  _userRepository.GetRoles();
        user.Countries = _countryRepository.GetCountries();
        user.States = _countryRepository.GetStates(user.CountryId);
        user.Cities = _countryRepository.GetCities(user.StateId);
        return user;
    }

    public EditUserViewModel GetUserAsync(long userId)
    {
        var user =  _userRepository.GetUserByIdAsync(userId);
        if (user == null)
            return null;

        user.Roles =  _userRepository.GetRoles();
        user.Countries = _countryRepository.GetCountries();
        user.States = _countryRepository.GetStates(user.CountryId);
        user.Cities = _countryRepository.GetCities(user.StateId);
        return user;
    }

    public async Task<bool> UpdateUser(EditUserViewModel model)
    {
        return await _userRepository.UpdateUser(model);
    }

#endregion

/*----------------------------------------------------------------Soft Delete User--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Soft Delete

    public async Task<bool> SoftDeleteUser(long id){
        return await _userRepository.SoftDeleteUser(id);
    }

#endregion

/*----------------------------------------------------------------Common--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Common
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        return user;
    }

    // public async Task<List<UserInfoViewModel>> GetUserInfoAsync()
    // {
    //     var userList = await _userRepository.GetUsersInfoAsync();
    //     return userList;
    // }


    public List<Role> GetRoles()
    {
        return _userRepository.GetRoles();
    }
#endregion

}
