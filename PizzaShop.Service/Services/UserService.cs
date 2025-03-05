using PizzaShop.Entity.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Helpers;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PizzaShop.Service.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IAddressService _addressService;
    private readonly IEmailService _emailService;

    public UserService(IGenericRepository<User> userRepository, IGenericRepository<Role> roleRepository, IAddressService addressService, IEmailService emailService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _addressService = addressService;
        _emailService = emailService;
    }

/*----------------------------------------------------------------Display User List--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Display User List

    // public UsersListViewModel GetUsersList(int pageNumber, int pageSize, string search)
    // {
    //     var userList =  _userRepository.GetPagedRecords(pageSize, pageNumber, search))
    //     return userList;
    // }

    public UsersListViewModel GetPagedRecords(int pageSize, int pageNumber)
    {
        UsersListViewModel model = new(){ Page = new() };
        var usersDb = _userRepository.GetPagedRecords(pageSize,pageNumber,orderBy: q => q.OrderBy(u => u.Id));
        model.Users = usersDb.records.Select(u => new UserInfoViewModel()
        {
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Phone = u.Phone,
            Role = "Chef",
            Status = u.IsActive,
            IsDeleted = u.IsDeleted,
            UserId = u.Id,
            ProfileImageUrl = u.ProfileImg
        }).ToList();

        model.Page.SetPagination(usersDb.totalRecord, pageSize, pageNumber);
        return model;
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
            Countries = _addressService.GetCountries(),
            Roles = _roleRepository.GetAll().ToList()
        };

        return newUser;
    }

    public async Task<bool> AddUserAsync(AddUserViewModel model, string createrEmail)
    {
        var creater = await _userRepository.GetByStringAsync(u => u.Email == createrEmail);

        var password = model.Password;
        model.Password = PasswordHelper.HashPassword(password);

        await _userRepository.AddAsync(new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Password = model.Password,
            CountryId = model.CountryId,
            StateId = model.StateId,
            CityId = model.CityId,
            RoleId = model.RoleId,
            CreatedBy = creater.Id
        });
    
        var success = true;
       
       if(true)
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

        return success;
    }
#endregion

/*----------------------------------------------------------------Edit User--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Edit User

    // public EditUserViewModel GetUserByIdAsync(long id)
    // {
    //     var user = _userRepository.GetUserByIdAsync(id);
    //     if (user == null)
    //         return null;

    //     user.Roles =  _userRepository.GetRoles();
    //     user.Countries = _countryRepository.GetCountries();
    //     user.States = _countryRepository.GetStates(user.CountryId);
    //     user.Cities = _countryRepository.GetCities(user.StateId);
    //     return user;
    // }

    // public EditUserViewModel GetUserAsync(long userId)
    // {
    //     var user =  _userRepository.GetUserByIdAsync(userId);
    //     if (user == null)
    //         return null;

    //     user.Roles =  _userRepository.GetRoles();
    //     user.Countries = _countryRepository.GetCountries();
    //     user.States = _countryRepository.GetStates(user.CountryId);
    //     user.Cities = _countryRepository.GetCities(user.StateId);
    //     return user;
    // }

    // public async Task<bool> UpdateUser(EditUserViewModel model)
    // {
    //     return await _userRepository.UpdateUser(model);
    // }

#endregion

/*----------------------------------------------------------------Soft Delete User--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Soft Delete

    // public async Task<bool> SoftDeleteUser(long id){
    //     return await _userRepository.SoftDeleteUser(id);
    // }

#endregion

/*----------------------------------------------------------------Common--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------*/
#region Common
    // public async Task<User?> GetUserByEmailAsync(string email)
    // {
    //     var user = await _userRepository.GetUserByEmailAsync(email);
    //     return user;
    // }

    // public async Task<List<UserInfoViewModel>> GetUserInfoAsync()
    // {
    //     var userList = await _userRepository.GetUsersInfoAsync();
    //     return userList;
    // }


    // public List<Role> GetRoles()
    // {
    //     return _userRepository.GetRoles();
    // }
#endregion
}
