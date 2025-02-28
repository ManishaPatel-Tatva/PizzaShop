using BusinessLogicLayer.Helpers;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessLogicLayer.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;
    private readonly ICountryRepository _countryRepository;
    public UserService(IUserRepository userRepository, JwtService jwtService, ICountryRepository countryRepository)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _countryRepository = countryRepository;
    }

    /*----------------------------------------------------------------Display User List--------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------*/
    #region Display User List

    public async Task<UsersListViewModel> GetUsersListAsync()
    {
        var users = await _userRepository.GetUsersInfoAsync();
        return new UsersListViewModel { User = users };
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

    public async Task AddUserAsync(AddUserViewModel model, string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Invalid token.");

        var email = _jwtService.GetClaimValue(token, "email");
        var creater = await GetUserByEmailAsync(email);

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Username = model.UserName,
            RoleId = model.RoleId,
            Email = model.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Phone = model.Phone,
            CountryId = model.CountryId,
            StateId = model.StateId,
            CityId = model.CityId,
            Address = model.Address,
            ZipCode = model.ZipCode,
            CreatedBy = creater.Id
        };

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

        try{
            await _userRepository.AddAsync(user);

            
            // string body = $@"
            //     <div style='background-color: #F2F2F2;'>
            //         <div style='background-color: #0066A8; color: white; height: 90px; font-size: 40px; font-weight: 600; text-align: center; padding-top: 40px; margin-bottom: 0px;'>PIZZASHOP</div>
            //         <div style='font-family:Verdana, Geneva, Tahoma, sans-serif; margin-top: 0px; font-size: 20px; padding: 10px;'>
            //             <p>Pizza shop,</p>
            //             <p>Please click <a href='{resetLink}'>here</a> for reset your account Password.</p>
            //             <p>If you encounter any issues or have any question, please do not hesitate to contact our support team.</p>
            //             <p><span style='color: orange;'>Important Note:</span> For security reasons, the link will expire in 24 hours. If you did not request a password reset, please ignore this email or contact our support team immediately.</p>
            //         </div>
            //     </div>";

            // await _emailService.SendEmailAsync(email, "Reset Password", body);
            
            // user.Email
        }
        catch(Exception){

        }
        


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
#region common
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        return user;
    }

    public async Task<List<UserInfoViewModel>> GetUserInfoAsync()
    {
        var userList = await _userRepository.GetUsersInfoAsync();
        return userList;
    }


    public List<Role> GetRoles()
    {
        return _userRepository.GetRoles();
    }
#endregion

}
